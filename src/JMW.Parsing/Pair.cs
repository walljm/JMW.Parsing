namespace JMW.Parsing;

public record Pair
{
    public string Key { get; init; }
    public string Value { get; init; }
    public IReadOnlyList<Pair> Children { get; init; }
    public ChildType ChildType { get; init; }

    public Pair(string key, string value, IReadOnlyList<Pair> children, ChildType childType = ChildType.StringType)
    {
        if (children.Count > 0 && childType == ChildType.StringType)
        {
            throw new ArgumentException(
                "Non-empty children requires explicit ChildType (ObjectType or ArrayType).",
                nameof(childType));
        }

        Key = key;
        Value = value;
        Children = children;
        ChildType = childType;
    }
}
