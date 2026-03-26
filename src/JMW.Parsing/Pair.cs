namespace JMW.Parsing;

public record Pair(string Key, string Value, IReadOnlyList<Pair> Children, ChildType ChildType = ChildType.StringType);
