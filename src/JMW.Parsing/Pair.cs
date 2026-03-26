namespace JMW.Parsing;

internal record Pair(string Key, string Value, IReadOnlyList<Pair> Children, ChildType ChildType = ChildType.StringType);
