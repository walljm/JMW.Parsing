namespace JMW.Parsing;

/// <summary>
/// Complete configuration for parsing a command's output.
/// Combines tokenizer rules, keyword schema, and output conventions.
/// </summary>
public sealed class ParserConfig
{
    /// <summary>How raw text is split into tokens.</summary>
    public required TokenizerConfig Tokenizer { get; init; }

    /// <summary>
    /// The keyword lookup table. Keys are the keyword strings that appear in the token stream.
    /// </summary>
    public required Dictionary<string, KeywordDef> Keywords { get; init; }

    /// <summary>
    /// Block splitting configuration.
    /// </summary>
    public bool TrimInitialWhitespace { get; init; } = true;
    public bool TrimEndingWhitespace { get; init; }

    /// <summary>
    /// Called to produce the initial Pair(s) from the first token(s) of a block.
    /// Return the pairs to yield and advance the enumerator as needed.
    /// If null, the first token is yielded as Pair("Name", firstToken, []).
    /// </summary>
    public Func<IEnumerator<string>, Queue<string>, IEnumerable<Pair>>? FirstTokenHandler { get; init; }

    /// <summary>
    /// Called before a block is parsed. Return true to parse the block, false to skip it.
    /// The string argument is the raw block text.
    /// If null, all blocks are parsed.
    /// </summary>
    public Func<string, bool>? BlockFilter { get; init; }

    /// <summary>
    /// Called when a block is skipped by BlockFilter. Receives the raw block text.
    /// Useful for extracting metadata (like ScutilDns DNS header tracking).
    /// </summary>
    public Action<string>? OnBlockSkipped { get; init; }

    /// <summary>
    /// Custom token post-processing hooks. Called for each token that doesn't match
    /// a keyword. Return pairs to yield, or empty to skip.
    /// If null, unrecognized tokens are silently dropped.
    /// </summary>
    public Func<string, IEnumerator<string>, Queue<string>, IEnumerable<Pair>>? UnknownTokenHandler { get; init; }

    /// <summary>
    /// Custom block splitter. When set, replaces the default Helpers.GetBlocks behavior.
    /// Receives the input TextReader and returns an enumerable of raw block strings.
    /// When set, TrimInitialWhitespace and TrimEndingWhitespace are ignored.
    /// </summary>
    public Func<TextReader, IEnumerable<string>>? BlockSplitter { get; init; }
}
