namespace JMW.Parsing;

/// <summary>
/// Defines how a keyword should be parsed when encountered in the token stream.
/// Replaces the separate GroupDefinition, MultipleDefinition, and TokenDef types
/// that were previously duplicated across parsers.
/// </summary>
public sealed class KeywordDef
{
    /// <summary>How the keyword's value is read from the stream.</summary>
    public KeywordKind Kind { get; init; } = KeywordKind.Next;

    /// <summary>
    /// Child keywords recognized when inside this keyword's group scope.
    /// Each child carries its own KeywordDef so it can have a CustomHandler,
    /// Kind, or other behavior independent of the top-level registry.
    /// Null means this is not a group keyword.
    /// </summary>
    public Dictionary<string, KeywordDef>? ChildKeywords { get; init; }

    /// <summary>
    /// Child keywords that themselves are multi-token (compound) keywords.
    /// Only relevant inside group scopes. Null means none.
    /// </summary>
    public Dictionary<string, MultiTokenDef>? ChildMultiTokenKeywords { get; init; }

    /// <summary>
    /// For multi-token (compound) keywords at the top level: the set of tokens
    /// that follow this keyword to form the compound key.
    /// E.g. "nd6" is followed by "options" → key becomes "nd6 options".
    /// Null means this is not a multi-token keyword.
    /// </summary>
    public MultiTokenDef? MultiToken { get; init; }

    /// <summary>
    /// When true, repeated occurrences of this keyword are collected into an array Pair.
    /// The array key defaults to "{keyword}s" unless ArrayKey is set.
    /// </summary>
    public bool IsArray { get; init; }

    /// <summary>
    /// Custom key name for the array wrapper Pair. When null, defaults to "{keyword}s".
    /// Use this to override naive pluralization (e.g. "addresses" instead of "addresss").
    /// </summary>
    public string? ArrayKey { get; init; }

    /// <summary>
    /// When true, the children of array items are merged into a single flat object
    /// instead of being nested. Used for RX/TX in ifconfig.
    /// </summary>
    public bool MergeChildren { get; init; }

    /// <summary>
    /// Optional custom handler for this keyword. When set, the engine delegates entirely
    /// to this function instead of the standard handling logic.
    /// Receives (token, enumerator, queue) and returns Pairs to yield.
    /// </summary>
    public Func<string, IEnumerator<string>, Queue<string>, IEnumerable<Pair>>? CustomHandler { get; init; }

    /// <summary>Whether this is a group keyword (has child keyword scope).</summary>
    public bool IsGroup => ChildKeywords is not null;
}

/// <summary>
/// Defines a multi-token (compound) keyword: a keyword that is followed by
/// additional tokens that form part of the key name.
/// </summary>
public sealed class MultiTokenDef
{
    /// <summary>How the final compound keyword's value is read.</summary>
    public required KeywordKind Kind { get; init; }

    /// <summary>
    /// The ordered sequence of following tokens expected.
    /// Token is the text to match, Kind is how to handle it (Drop = consume but omit from key name).
    /// Order matters: tokens are matched in sequence.
    /// </summary>
    public required IReadOnlyList<(string Token, KeywordKind Kind)> Keywords { get; init; }
}
