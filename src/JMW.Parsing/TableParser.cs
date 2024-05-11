#nullable disable

using System.Collections.Generic;
using JMW.Parsing;

namespace VAE.Common.Parsing.Tables;

public class CellsData
{
    public ColumnRowData ParsingData { get; set; }

    public List<string> ColumnNames { get; set; } = [];

    public List<List<string>> Rows { get; set; } = [];
}

public class ColumnRowData
{
    public Dictionary<char, int> CandidateSeparators { get; set; } = new ();
    public int RowStart { get; set; } = 0;
    public int RowCount { get; set; } = 0;

    public bool IsTable
    {
        get { return CandidateSeparators.Count > 0 && RowCount > 1; }
    }
}

public class ColumnPositionCollection
{
    public ColumnPositionCollection(Dictionary<string, ColumnPosition> positions)
    {
        Positions = positions;
        var start = -1;
        foreach (var pos in positions)
        {
            if (pos.Value.Start > start) start = pos.Value.Start;
        }

        LastStartPos = start;
    }

    public Dictionary<string, ColumnPosition> Positions { get ; set ; }

    public int LastStartPos { get ; }
}

public class ColumnPosition
{
    public string Name { get ; set ; } = "";

    public string Key { get ; set ; } = "";

    public int Start { get ; set ; } = -1;

    public int Length { get ; set ; } = -1;

    public override string ToString()
    {
        if (Length == -1) return Name + ": " + Start;
        else if (Length == 0) return Name + ": " + Start + "-" + Start;
        return Name + ": " + Start + "-" + (Start + Length - 1);
    }
}

public class FixedWidthTableParser
{
    public bool VerifyColumns { get; set; } = true;
    public string ColumnHeaderSplitString { get; set; } = " ";

    public List<List<string>> ParseCells(IList<string> lines)
    {
        var td = new CellsData();
        var colPositions = lines.DeriveColumnPositions(VerifyColumns, ColumnHeaderSplitString);

        for (var i = 1; i < lines.Count; i++)
        {
            var line = lines[i];

            var hist = line.GetCharHistogram();

            // ignore lines that don't have more than 2 chars (those are hrules)
            if (hist.Count > 2)
            {
                var cols = new List<string>();
                foreach (var pair in colPositions.Positions)
                {
                    cols.Add(colPositions.GetColumnValue(line, pair.Key));
                }

                td.Rows.Add(cols);
            }
        }

        return td.Rows;
    }
}

public static class Helpers
{
    /// <summary>
    ///   Assumes the first row of the table is the column row.  Derives the column positions.  Handles whitespace and paging breaks in the table.
    /// </summary>
    public static ColumnPositionCollection DeriveColumnPositions(
        this IList<string> table,
        bool validate_with_data = true,
        string columnHeaderSplitString = " "
    )
    {
        var whitespace = new HashSet<string>() { " ", "\r", "\n", "+" };

        #region Get Columns

        // find the first non empty line.
        var c = 0;
        var columnnames = table[c];
        while (columnnames.Trim().Length == 0)
        {
            columnnames = table[++c];
        }

        // initialize the field positions dictionary object.
        var field_positions = new Dictionary<string, ColumnPosition>();
        var positions = new List<ColumnPosition>();

        // get the field names.  we assume the line has them.
        var pre_columns = columnnames.Trim().Split([ columnHeaderSplitString ], StringSplitOptions.RemoveEmptyEntries);

        // handle columnnames
        columnnames = HandleDuplicateColumnNames(columnnames, pre_columns);
        table[c] = columnnames;
        var columns = columnnames.Trim().Split([ columnHeaderSplitString ], StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();

        for (var i = 0; i < columns.Length; i++)
        {
            var col = columns[i];

            var o = new ColumnPosition();

            if (i + 1 == columns.Length) // we're on the last one
            {
                o.Start = columnnames.LastIndexOf(" " + col, StringComparison.Ordinal) + 1;
                o.Length = -1;
            }
            else if (i + 2 == columns.Length) // we're on the second to last, but that means the next column is the last.
            {
                o.Start = columnnames.IndexOf(" " + col + " ", StringComparison.Ordinal) + 1;
                o.Length = columnnames.LastIndexOf(" " + columns[i + 1], StringComparison.Ordinal) - o.Start + 1;
            }
            else if (i == 0) // we're on the first, treat the start special.
            {
                o.Start = columnnames.IndexOf(col + " ", StringComparison.Ordinal);
                o.Length = columnnames.IndexOf(" " + columns[i + 1] + " ", StringComparison.Ordinal) - o.Start + 1;
            }
            else
            {
                o.Start = columnnames.IndexOf(" " + col + " ", StringComparison.Ordinal) + 1;
                o.Length = columnnames.IndexOf(" " + columns[i + 1] + " ", StringComparison.Ordinal) - o.Start + 1;
            }

            o.Name = pre_columns[i];
            o.Key = columns[i];
            field_positions.Add(col, o);
            positions.Add(o);
        }

        #endregion Get Columns

        #region Validate

        if (validate_with_data)
        {
            // we need to access the keys line a normal array/list and the key collection in the dict object wont let us.
            var keys = positions.Select(o => o.Key).ToList();

            //  we increase the start position at least one to hedge our bets, as we can rely on there being
            //   at least 1 char in the column for the row, and if there isn't, it doesn't matter anyway.
            //   but this does allow for situations where the header is off by one, because that happens.
            foreach (var pos in positions)
            {
                pos.Start++;
            }

            // now test to make sure your column boundaries are correct by looping through the rest of the table
            for (var i = c + 1; i < table.Count; i++)
            {
                var row = table[i];

                if (row.Trim().GetCharHistogram().Count > 1) // don't bother with empty lines.
                {
                    //check to see if the first column starts at the beginning of the line.
                    while (
                        field_positions[keys[0]].Start != 0
                     && row.Substring(field_positions[keys[0]].Start - 1, 1) != " "
                     && row.Substring(field_positions[keys[0]].Start - 1, 1) != "+"
                    )
                    {
                        field_positions[keys[0]].Start--;
                        field_positions[keys[0]].Length++;
                    }

                    // Adjust the rows if necessary.
                    // we ignore the last column, because its starting index will get moved, but its length is -1.
                    for (var j = 0; j < keys.Count - 1; j++)
                    {
                        var key = keys[j];

                        var start = field_positions[key].Start + field_positions[key].Length - 1;

                        while (start >= 0 && row.Length > start && !whitespace.Contains(row.Substring(start, 1)))
                        {
                            field_positions[key].Length--; // only decrement the length if its not the last column
                            field_positions[keys[j + 1]].Start--; // only decrement the starting index of the next column if its not the last column

                            // now deal with the increased col length.
                            if (j + 2 < keys.Count) // its not the second to last
                            {
                                field_positions[keys[j + 1]]
                                   .Length++; // only increment the length of the next column if the next colum isn't the last column
                            }

                            start = field_positions[key].Start + field_positions[key].Length - 1; // reset the start pos.
                        }
                    }
                }
            }
        }

        field_positions = CombineMultiwordColumnNames(positions);

        #endregion Validate

        return new ColumnPositionCollection(field_positions);
    }

    public static Dictionary<string, ColumnPosition> CombineMultiwordColumnNames(List<ColumnPosition> positions)
    {
        // look for columns with a length of 1, as that column is basically a zero length column and should be combined with the column next to it.
        var field_positions = new Dictionary<string, ColumnPosition>();
        for (var i = 0; i < positions.Count; i++)
        {
            var p = positions[i]; // get the position
            if (p.Length == 0)
            {
                // the length of the position is 0, so it needs to be added to the column next to it.
                positions[i + 1].Name = positions[i].Name + positions[i + 1].Name; // merge this position with the next position.

                while (positions.Count > i + 1 && positions[i + 1].Length == 0)
                {
                    i++; // go to the next line.
                    positions[i + 1].Name = positions[i].Name + positions[i + 1].Name; // merge this position with the next position.
                }

                field_positions.Add(positions[i + 1].Name, positions[i + 1]); // add the next position to the fields.

                if (positions.Count > i + 1)
                {
                    i++;
                }
                else
                {
                    break;
                }
            }
            else
            {
                var id = p.Name;
                var increment = 0;
                while (field_positions.ContainsKey(id))
                {
                    // deal with the old, so the unit test doesn't break
                    var len = id.Length;
                    var old = field_positions[id];
                    field_positions.Remove(id);
                    id = p.Name.Substring(0, len - increment.ToString().Length) + increment;
                    while (field_positions.ContainsKey(id))
                    {
                        increment++;
                        id = p.Name.Substring(0, len - increment.ToString().Length) + increment;
                    }

                    field_positions.Add(id, old);

                    increment++;
                    len = p.Name.Length;
                    id = p.Name.Substring(0, len - increment.ToString().Length) + increment;
                    increment++;
                }

                field_positions.Add(id, p);
            }
        }

        return field_positions;
    }

    public static string HandleDuplicateColumnNames(string columnnames, string[] columns)
    {
        // now check for uniqueness.
        var cols = columns.Distinct().ToArray();

        foreach (var col in cols)
        {
            int occurrences = columnnames.CountInstances(col, true);
            if (occurrences > 1)
            {
                var n = 0;
                while (columnnames.Contains(col))
                {
                    var nc = columnnames.ParseToIndexOf(col)
                           + col.Substring(0, col.Length - n.ToString().Length)
                           + n
                           + columnnames.ParseAfterLastIndexOf_PlusLength(col);
                    columnnames = nc;
                    n++;
                }
            }
        }

        return columnnames;
    }

    public static string GetColumnValue(this ColumnPositionCollection positions, string line, string col)
    {
        var pos = positions.Positions;

        if (line.Length >= pos[col].Start + pos[col].Length && pos[col].Length > 0)
        {
            return line.Substring(pos[col].Start, pos[col].Length);
        }
        else if (line.Length > pos[col].Start)
        {
            return line.Substring(pos[col].Start);
        }
        else
        {
            return "";
        }
    }
}