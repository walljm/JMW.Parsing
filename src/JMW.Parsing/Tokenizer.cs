using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace JMW.Parsing;

public enum TokenTypes
{
    Word,
    Whitespace,
}

public record BlockData(HashSet<string> CommonTokens, List<string[]> Blocks);

public class Tokenizer
{
    private static TextParser<char> AnyNoneWhitespaceChar { get; } =
        Character.Matching(
            c =>
                !char.IsWhiteSpace(c)
            //&& !char.IsPunctuation(c)
            //&& c != ')'
            //&& c != '('
           ,
            "any non whitespace char"
        );

    private static TextParser<TextSpan> TextMatcher { get; } =
        Span.MatchedBy(AnyNoneWhitespaceChar.Many());

    private static readonly Tokenizer<TokenTypes> tokenizer;

    static Tokenizer()
    {
        tokenizer = new TokenizerBuilder<TokenTypes>()
           .Ignore(Character.WhiteSpace)
            //.Ignore(Character.Matching(c => char.IsPunctuation(c), "punctuation"))
            //.Ignore(Character.Matching(c => c == ')' || c == '(', "parens"))
           .Match(TextMatcher, TokenTypes.Word)
           .Build();
    }

    public static IEnumerable<Token<TokenTypes>> Tokenize(string text)
    {
        return tokenizer.Tokenize(text);
    }

    private static IEnumerable<IGrouping<int, string>> GetGroups(IEnumerable<string> lst, double factor = 1)
    {
        return lst.GroupBy(o => (int)Math.Floor(o.Length / factor) * Convert.ToInt32(factor));
    }

    public static BlockData GetBlockData(TextReader reader)
    {
        var blocks = Helpers.GetBlocks(reader);
        var groups = GetGroups(blocks, 500);

        return groups.Select(CommonTokens)
           .Aggregate(
                (p, c) =>
                {
                    p.CommonTokens.UnionWith(c.CommonTokens);
                    p.Blocks.AddRange(c.Blocks);
                    return p;
                }
            );
    }

    private static BlockData CommonTokens(IEnumerable<string> blocks)
    {
        var blockTokens = new List<string[]>();
        HashSet<string>? common = null;
        foreach (var block in blocks)
        {
            if (block.Length < 20)
            {
                continue;
            }

            var tokens = tokenizer.Tokenize(block).Select(o => o.ToStringValue()).ToArray();
            blockTokens.Add(tokens);
            if (common is null)
            {
                common = new HashSet<string>(tokens);
                continue;
            }

            common.IntersectWith(tokens);
        }

        return new BlockData(common ?? [], blockTokens);
    }
}