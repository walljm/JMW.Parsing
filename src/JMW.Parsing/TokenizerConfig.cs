namespace JMW.Parsing;

/// <summary>
/// Declarative configuration for splitting raw text into a stream of string tokens.
/// </summary>
public sealed class TokenizerConfig
{
    /// <summary>
    /// Characters that act as token separators AND are consumed (not included in token output).
    /// E.g. ':', '[', ']' for scutil; '=' for ifconfig.
    /// </summary>
    public required HashSet<char> Separators { get; init; }

    /// <summary>
    /// When a token ends with one of these characters, the character is stripped before yielding.
    /// E.g. ':' for ifconfig (strips trailing colons from identifiers).
    /// </summary>
    public HashSet<char> StripTrailing { get; init; } = [];

    /// <summary>
    /// When true, tokens containing exactly one ':' are split into two tokens.
    /// Used by ifconfig for "key:value" pairs like "txqueuelen:1000".
    /// </summary>
    public bool SplitOnSingleColon { get; init; }

    /// <summary>
    /// When true, yields a NewLine sentinel token on '\n'.
    /// Both parsers currently need this.
    /// </summary>
    public bool EmitNewLineTokens { get; init; } = true;

    /// <summary>
    /// When true, the tokenizer tracks leading whitespace on each line and
    /// produces <see cref="Token"/> values with a <see cref="Token.Depth"/> property.
    /// Use <see cref="BlockParser.TokenizeWithDepth"/> to obtain depth-annotated tokens,
    /// or set this flag and let <see cref="BlockParser.ParseBlock(string, ParserConfig)"/>
    /// route automatically to the depth-based tree builder.
    /// </summary>
    public bool MeasureIndentation { get; init; }
}
