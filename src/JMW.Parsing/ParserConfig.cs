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
    /// Open/close characters for Options-style keyword values.
    /// E.g. ('<', '>') for ifconfig flags, ('(', ')') for scutil reach.
    /// </summary>
    public (char Open, char Close) OptionDelimiters { get; init; } = ('<', '>');

    /// <summary>
    /// When true, Options-kind keywords read until newline instead of reading a single next token.
    /// ScutilDns needs this; ifconfig does not.
    /// </summary>
    public bool OptionsReadUntilNewLine { get; init; }

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
    /// Whether Table output is supported for this parser.
    /// </summary>
    public bool SupportsTableOutput { get; init; }

    /// <summary>
    /// Message to show when table output is requested but not supported.
    /// </summary>
    public string? TableNotSupportedMessage { get; init; }
}
