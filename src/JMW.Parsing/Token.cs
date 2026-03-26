namespace JMW.Parsing;

/// <summary>
/// A token with associated indentation depth. Produced by
/// <see cref="BlockParser.TokenizeWithDepth"/> when
/// <see cref="TokenizerConfig.MeasureIndentation"/> is enabled.
/// </summary>
public readonly record struct Token(string Value, int Depth);
