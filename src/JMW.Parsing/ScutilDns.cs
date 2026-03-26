namespace JMW.Parsing;

public static class ScutilDns
{
    #region Public Parse/Output

    public static void OutputKeyValues(TextReader inputReader, TextWriter outputWriter)
    {
        PairWriter.WriteKeyValues(BlockParser.ParseBlocks(inputReader, BuildConfig()), outputWriter);
    }

    public static void OutputJson(TextReader inputReader, TextWriter outputWriter)
    {
        PairWriter.WriteJson(BlockParser.ParseBlocks(inputReader, BuildConfig()), outputWriter);
    }

    #endregion

    #region Parser Configuration

    internal static ParserConfig BuildConfig()
    {
        // Per-call mutable state for the DNS header that precedes each resolver block
        var header = string.Empty;

        return new ParserConfig
        {
            Tokenizer = new TokenizerConfig
            {
                Separators = [':', '[', ']'],
                EmitNewLineTokens = true,
            },

            TrimInitialWhitespace = true,
            TrimEndingWhitespace = false,

            BlockFilter = block => !block.StartsWith("DNS"),
            OnBlockSkipped = block => header = block.Trim(),

            FirstTokenHandler = (enumerator, queue) =>
            {
                // First token should be "resolver"; skip it and read the resolver number
                if (enumerator.Current == "resolver")
                {
                    enumerator.MoveNext();
                }
                else
                {
                    return [];
                }

                return
                [
                    new Pair("Type", header, []),
                    new Pair("Resolver", enumerator.Current.Trim('#'), []),
                ];
            },

            Keywords = BuildKeywords(),
        };
    }

    private static Dictionary<string, KeywordDef> BuildKeywords()
    {
        return new Dictionary<string, KeywordDef>
        {
            { "nameserver", new KeywordDef { Kind = KeywordKind.Next, IsArray = true } },
            { "if_index", new KeywordDef { Kind = KeywordKind.NewLine } },
            { "flags", new KeywordDef { Kind = KeywordKind.NewLine } },
            { "reach", new KeywordDef { CustomHandler = KeywordHandlers.Options('(', ')', readUntilNewLine: true) } },
            { "domain", new KeywordDef { Kind = KeywordKind.Next } },
            { "options", new KeywordDef { Kind = KeywordKind.Next } },
            { "timeout", new KeywordDef { Kind = KeywordKind.Next } },
            { "order", new KeywordDef { Kind = KeywordKind.Next } },
            {
                "search", new KeywordDef
                {
                    Kind = KeywordKind.Next,
                    IsArray = true,
                    MultiToken = new MultiTokenDef
                    {
                        Kind = KeywordKind.Next,
                        Keywords = [("domain", KeywordKind.Next)],
                    },
                }
            },
        };
    }

    #endregion
}
