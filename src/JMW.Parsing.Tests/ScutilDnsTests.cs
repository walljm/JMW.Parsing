using Xunit.Abstractions;

namespace JMW.Parsing.Tests;

public class ScutilDnsTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void ParseJsonMacOsTest()
    {
        const string ifconfigOutput = @"
DNS configuration

resolver #1
  search domain[0] : home
  nameserver[0] : 192.168.1.54
  nameserver[1] : 192.168.1.52
  if_index : 24 (en7)
  flags    : Request A records
  reach    : 0x00020002 (Reachable,Directly Reachable Address)

resolver #2
  domain   : local
  options  : mdns
  timeout  : 5
  flags    : Request A records
  reach    : 0x00000000 (Not Reachable)
  order    : 300000

resolver #3
  domain   : 254.169.in-addr.arpa
  options  : mdns
  timeout  : 5
  flags    : Request A records
  reach    : 0x00000000 (Not Reachable)
  order    : 300200

resolver #4
  domain   : 8.e.f.ip6.arpa
  options  : mdns
  timeout  : 5
  flags    : Request A records
  reach    : 0x00000000 (Not Reachable)
  order    : 300400

resolver #5
  domain   : 9.e.f.ip6.arpa
  options  : mdns
  timeout  : 5
  flags    : Request A records
  reach    : 0x00000000 (Not Reachable)
  order    : 300600

resolver #6
  domain   : a.e.f.ip6.arpa
  options  : mdns
  timeout  : 5
  flags    : Request A records
  reach    : 0x00000000 (Not Reachable)
  order    : 300800

resolver #7
  domain   : b.e.f.ip6.arpa
  options  : mdns
  timeout  : 5
  flags    : Request A records
  reach    : 0x00000000 (Not Reachable)
  order    : 301000

DNS configuration (for scoped queries)

resolver #1
  search domain[0] : home
  nameserver[0] : 192.168.1.54
  nameserver[1] : 192.168.1.52
  if_index : 24 (en7)
  flags    : Scoped, Request A records
  reach    : 0x00020002 (Reachable,Directly Reachable Address)

resolver #2
  search domain[0] : home
  nameserver[0] : 192.168.1.54
  nameserver[1] : 192.168.1.52
  if_index : 15 (en0)
  flags    : Scoped, Request A records
  reach    : 0x00020002 (Reachable,Directly Reachable Address)
";

		    const string expectedJson = @"[
  {
    ""Type"": ""DNS configuration"",
    ""Resolver"": ""1"",
    ""SearchDomains"": [
      ""home""
    ],
    ""Nameservers"": [
      ""192.168.1.54"",
      ""192.168.1.52""
    ],
    ""Ifindex"": ""24 (en7)"",
    ""Flags"": ""Request A records"",
    ""Reach"": {
      ""Bits"": "" 0x00020002 "",
      ""Values"": [
        ""Reachable"",
        ""Directly Reachable Address""
      ]
    }
  },
  {
    ""Type"": ""DNS configuration"",
    ""Resolver"": ""2"",
    ""Domain"": ""local"",
    ""Options"": ""mdns"",
    ""Timeout"": ""5"",
    ""Flags"": ""Request A records"",
    ""Reach"": {
      ""Bits"": "" 0x00000000 "",
      ""Values"": [
        ""Not Reachable""
      ]
    },
    ""Order"": ""300000""
  },
  {
    ""Type"": ""DNS configuration"",
    ""Resolver"": ""3"",
    ""Domain"": ""254.169.in-addr.arpa"",
    ""Options"": ""mdns"",
    ""Timeout"": ""5"",
    ""Flags"": ""Request A records"",
    ""Reach"": {
      ""Bits"": "" 0x00000000 "",
      ""Values"": [
        ""Not Reachable""
      ]
    },
    ""Order"": ""300200""
  },
  {
    ""Type"": ""DNS configuration"",
    ""Resolver"": ""4"",
    ""Domain"": ""8.e.f.ip6.arpa"",
    ""Options"": ""mdns"",
    ""Timeout"": ""5"",
    ""Flags"": ""Request A records"",
    ""Reach"": {
      ""Bits"": "" 0x00000000 "",
      ""Values"": [
        ""Not Reachable""
      ]
    },
    ""Order"": ""300400""
  },
  {
    ""Type"": ""DNS configuration"",
    ""Resolver"": ""5"",
    ""Domain"": ""9.e.f.ip6.arpa"",
    ""Options"": ""mdns"",
    ""Timeout"": ""5"",
    ""Flags"": ""Request A records"",
    ""Reach"": {
      ""Bits"": "" 0x00000000 "",
      ""Values"": [
        ""Not Reachable""
      ]
    },
    ""Order"": ""300600""
  },
  {
    ""Type"": ""DNS configuration"",
    ""Resolver"": ""6"",
    ""Domain"": ""a.e.f.ip6.arpa"",
    ""Options"": ""mdns"",
    ""Timeout"": ""5"",
    ""Flags"": ""Request A records"",
    ""Reach"": {
      ""Bits"": "" 0x00000000 "",
      ""Values"": [
        ""Not Reachable""
      ]
    },
    ""Order"": ""300800""
  },
  {
    ""Type"": ""DNS configuration"",
    ""Resolver"": ""7"",
    ""Domain"": ""b.e.f.ip6.arpa"",
    ""Options"": ""mdns"",
    ""Timeout"": ""5"",
    ""Flags"": ""Request A records"",
    ""Reach"": {
      ""Bits"": "" 0x00000000 "",
      ""Values"": [
        ""Not Reachable""
      ]
    },
    ""Order"": ""301000""
  },
  {
    ""Type"": ""DNS configuration (for scoped queries)"",
    ""Resolver"": ""1"",
    ""SearchDomains"": [
      ""home""
    ],
    ""Nameservers"": [
      ""192.168.1.54"",
      ""192.168.1.52""
    ],
    ""Ifindex"": ""24 (en7)"",
    ""Flags"": ""Scoped, Request A records"",
    ""Reach"": {
      ""Bits"": "" 0x00020002 "",
      ""Values"": [
        ""Reachable"",
        ""Directly Reachable Address""
      ]
    }
  },
  {
    ""Type"": ""DNS configuration (for scoped queries)"",
    ""Resolver"": ""2"",
    ""SearchDomains"": [
      ""home""
    ],
    ""Nameservers"": [
      ""192.168.1.54"",
      ""192.168.1.52""
    ],
    ""Ifindex"": ""15 (en0)"",
    ""Flags"": ""Scoped, Request A records"",
    ""Reach"": {
      ""Bits"": "" 0x00020002 "",
      ""Values"": [
        ""Reachable"",
        ""Directly Reachable Address""
      ]
    }
  }
]
";

        using var reader = new StringReader(ifconfigOutput);
        using var writer = new StringWriter();
        ScutilDns.Parse(reader, writer, new ParsingOptions(OutputType.Json));

        var actualOutput = writer.ToString();
        testOutputHelper.WriteLine(actualOutput);
        Assert.Equal(expectedJson.Replace("\r\n", "\n"), actualOutput.Replace("\r\n", "\n"));
    }
}