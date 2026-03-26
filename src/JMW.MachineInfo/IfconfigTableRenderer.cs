using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using JMW.Parsing;
using table.lib;

namespace JMW.MachineInfo;

/// <summary>
/// Renders ifconfig parsed output as a table. This is a presentation concern
/// that lives in the CLI project, not in the parsing library.
/// </summary>
public static class IfconfigTableRenderer
{
    public static void OutputTable(TextReader inputReader, TextWriter outputWriter, DisplayOptions displayOptions)
    {
        #region Convert blocks to pocos

        var data = new List<Ifc>();
        foreach (var pairs in BlockParser.ParseBlocks(inputReader, Ifconfig.Config))
        {
            var ifc = new Ifc();

            foreach (var pair in pairs)
            {
                if (pair.Key == "InterfaceName")
                {
                    ifc.Name = pair.Value;
                }
                else if (pair.Key == "status")
                {
                    ifc.Status = pair.Value;
                }
                else if (pair.Key == "flags")
                {
                    ifc.Flags = string.Join(
                        ',',
                        pair.Children.FirstOrDefault(static o => o.Key == "Values")?.Children.Select(static o => o.Value) ?? []
                    );
                    ifc.AdminStatus =
                        pair.Children.FirstOrDefault(static o => o.Key == "Values")?.Children.FirstOrDefault(static o => o.Value == "UP") is not null
                            ? "Up"
                            : "Down";
                    ifc.OperStatus = pair.Children
                       .FirstOrDefault(static o => o.Key == "Values")
                      ?.Children
                       .FirstOrDefault(static o => o.Value == "RUNNING") is not null
                        ? "Up"
                        : "Down";
                }
                else if (pair.Key == "index")
                {
                    ifc.Index = int.TryParse(pair.Value, out var idx) ? idx : -1;
                }
                else if (pair.Key == "type")
                {
                    ifc.Type = pair.Value;
                }
                else if (pair.Key == "ether")
                {
                    ifc.MAC = pair.Value;
                }
                else if (pair.Key == "inets")
                {
                    ifc.IP = string.Join(
                        ", ",
                        pair.Children.Select(
                            static o => string.Join(
                                    '/',
                                    o.Children.Select(
                                        static i =>
                                        {
                                            if (i.Key == "netmask")
                                            {
                                                return GetMaskLen(i.Value).ToString();
                                            }
                                            else if (i.Key == "inet")
                                            {
                                                return i.Value;
                                            }

                                            return string.Empty;
                                        }
                                    )
                                )
                               .TrimEnd('/')
                        )
                    );
                }
                else if (pair.Key == "media")
                {
                    ifc.Media = pair.Value;
                }
            }

            if (ifc.Type.Length == 0)
            {
                if (ifc.Flags.Contains("POINTOPOINT"))
                {
                    ifc.Type = "Tunnel";
                }
                else if (ifc.Name.StartsWith("bridge"))
                {
                    ifc.Type = "Virtual Bridge";
                }
                else if (ifc.Name.StartsWith("stf"))
                {
                    ifc.Type = "6to4 Tunnel";
                }
                else if (ifc.Flags.Contains("LOOPBACK"))
                {
                    ifc.Type = "Loopback";
                }
            }

            data.Add(ifc);
        }

        #endregion

        #region Filter data and handle display width

        if (displayOptions.Filter is not null)
        {
            Regex regex;
            try
            {
                var opts = RegexOptions.IgnoreCase | RegexOptions.NonBacktracking | RegexOptions.CultureInvariant;
                regex = new Regex(displayOptions.Filter, opts);
            }
            catch (RegexParseException ex)
            {
                Console.Error.WriteLine($"Invalid filter regex: {ex.Message}");
                return;
            }

            data = data.Where(
                    o => regex.IsMatch(o.Status)
                      || regex.IsMatch(o.Name)
                      || regex.IsMatch(o.IP)
                      || regex.IsMatch(o.MAC)
                      || regex.IsMatch(o.AdminStatus)
                      || regex.IsMatch(o.OperStatus)
                )
               .ToList();
        }

        data = data.OrderBy(static o => o.Index).ThenBy(static o => o.Name).ToList();
        var columns = new List<string>
        {
            nameof(Ifc.Name),
            nameof(Ifc.Status),
            nameof(Ifc.Flags),
            nameof(Ifc.AdminStatus),
            nameof(Ifc.OperStatus),
            nameof(Ifc.Index),
            nameof(Ifc.Type),
            nameof(Ifc.MAC),
            nameof(Ifc.IP),
            nameof(Ifc.Media),
        };

        if (displayOptions.ConsoleWidth < 225)
        {
            columns.Remove(nameof(Ifc.Media));
        }

        if (displayOptions.ConsoleWidth < 180)
        {
            columns.Remove(nameof(Ifc.Flags));
        }

        if (displayOptions.ConsoleWidth < 120)
        {
            columns.Remove(nameof(Ifc.Index));
        }

        if (displayOptions.ConsoleWidth < 100)
        {
            columns.Remove(nameof(Ifc.AdminStatus));
        }

        #endregion

        var tbl = Table<Ifc>.Add(data);
        tbl.FilterColumns(columns.ToArray(), FilterAction.Include);
        var result = tbl.ToMarkDown();
        outputWriter.Write(result);
    }

    internal static int GetMaskLen(string mask)
    {
        byte[] bytes;

        if (mask.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
         && uint.TryParse(mask.AsSpan(2), NumberStyles.HexNumber, null, out var hex))
        {
            bytes =
            [
                (byte)(hex >> 24),
                (byte)(hex >> 16),
                (byte)(hex >> 8),
                (byte)hex,
            ];
        }
        else if (IPAddress.TryParse(mask, out var ipAddress))
        {
            bytes = ipAddress.GetAddressBytes();
        }
        else
        {
            return 0;
        }

        var cidr = 0;
        foreach (var b in bytes)
        {
            for (var bit = 7; bit >= 0; bit--)
            {
                if ((b & (1 << bit)) != 0)
                {
                    cidr++;
                }
                else
                {
                    return cidr;
                }
            }
        }

        return cidr;
    }

    internal class Ifc
    {
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Flags { get; set; } = string.Empty;
        public string AdminStatus { get; set; } = string.Empty;
        public string OperStatus { get; set; } = string.Empty;
        public int Index { get; set; } = -1;
        public string Type { get; set; } = string.Empty;
        public string MAC { get; set; } = string.Empty;
        public string IP { get; set; } = string.Empty;
        public string Media { get; set; } = string.Empty;
    }
}
