namespace JMW.Parsing.Tests;

public class BlockParserTests
{
    #region Tokenize Tests

    [Fact]
    public void TokenizeBasicWhitespaceSeparation()
    {
        var config = new TokenizerConfig { Separators = [] };
        using var reader = new StringReader("hello world foo");
        var tokens = BlockParser.Tokenize(reader, config).ToList();
        Assert.Equal(["hello", "world", "foo"], tokens);
    }

    [Fact]
    public void TokenizeSeparatorChars()
    {
        var config = new TokenizerConfig { Separators = [':'] };
        using var reader = new StringReader("key:value");
        var tokens = BlockParser.Tokenize(reader, config).ToList();
        Assert.Equal(["key", "value"], tokens);
    }

    [Fact]
    public void TokenizeStripTrailing()
    {
        var config = new TokenizerConfig { Separators = ['='], StripTrailing = [':'] };
        using var reader = new StringReader("key: =value");
        var tokens = BlockParser.Tokenize(reader, config).ToList();
        Assert.Equal(["key", "value"], tokens);
    }

    [Fact]
    public void TokenizeNewLineEmission()
    {
        var config = new TokenizerConfig { Separators = [], EmitNewLineTokens = true };
        using var reader = new StringReader("line1\nline2");
        var tokens = BlockParser.Tokenize(reader, config).ToList();
        Assert.Equal(["line1", Helpers.NewLine, "line2"], tokens);
    }

    [Fact]
    public void TokenizeNoNewLineEmission()
    {
        var config = new TokenizerConfig { Separators = [], EmitNewLineTokens = false };
        using var reader = new StringReader("line1\nline2");
        var tokens = BlockParser.Tokenize(reader, config).ToList();
        Assert.Equal(["line1", "line2"], tokens);
    }

    [Fact]
    public void TokenizeSplitOnSingleColon()
    {
        var config = new TokenizerConfig { Separators = [], SplitOnSingleColon = true };
        using var reader = new StringReader("key:value no:split:here");
        var tokens = BlockParser.Tokenize(reader, config).ToList();
        // Single colon splits; multiple colons do not
        Assert.Equal(["key", "value", "no:split:here"], tokens);
    }

    [Fact]
    public void TokenizeMultipleSeparators()
    {
        var config = new TokenizerConfig { Separators = [':', '=', '[', ']'] };
        using var reader = new StringReader("a:b=c[d]e");
        var tokens = BlockParser.Tokenize(reader, config).ToList();
        Assert.Equal(["a", "b", "c", "d", "e"], tokens);
    }

    #endregion

    #region ParseBlocks Tests

    [Fact]
    public void ParseBlocksMinimalSchema()
    {
        var input = "block1\n  key1 val1\n  key2 val2\nblock2\n  key1 val3";

        var config = new ParserConfig
        {
            Tokenizer = new TokenizerConfig { Separators = [], EmitNewLineTokens = true },
            Keywords = new()
            {
                { "key1", new KeywordDef { Kind = KeywordKind.Next } },
                { "key2", new KeywordDef { Kind = KeywordKind.Next } },
            },
        };

        using var reader = new StringReader(input);
        var blocks = BlockParser.ParseBlocks(reader, config).ToList();

        Assert.Equal(2, blocks.Count);

        // First block
        Assert.Equal("Name", blocks[0][0].Key);
        Assert.Equal("block1", blocks[0][0].Value);
        Assert.Equal("key1", blocks[0][1].Key);
        Assert.Equal("val1", blocks[0][1].Value);
        Assert.Equal("key2", blocks[0][2].Key);
        Assert.Equal("val2", blocks[0][2].Value);

        // Second block
        Assert.Equal("Name", blocks[1][0].Key);
        Assert.Equal("block2", blocks[1][0].Value);
        Assert.Equal("key1", blocks[1][1].Key);
        Assert.Equal("val3", blocks[1][1].Value);
    }

    [Fact]
    public void ParseBlocksWithBlockFilter()
    {
        var input = "HEADER line\nblock1\n  key1 val1";

        string? skipped = null;
        var config = new ParserConfig
        {
            Tokenizer = new TokenizerConfig { Separators = [], EmitNewLineTokens = true },
            Keywords = new()
            {
                { "key1", new KeywordDef { Kind = KeywordKind.Next } },
            },
            BlockFilter = block => !block.StartsWith("HEADER"),
            OnBlockSkipped = block => skipped = block.Trim(),
        };

        using var reader = new StringReader(input);
        var blocks = BlockParser.ParseBlocks(reader, config).ToList();

        Assert.Single(blocks);
        Assert.Equal("HEADER line", skipped);
    }

    [Fact]
    public void ParseBlocksWithCustomBlockSplitter()
    {
        var input = "key1 val1\n---\nkey1 val2";

        var config = new ParserConfig
        {
            Tokenizer = new TokenizerConfig { Separators = [], EmitNewLineTokens = true },
            Keywords = new()
            {
                { "key1", new KeywordDef { Kind = KeywordKind.Next } },
            },
            BlockSplitter = reader =>
            {
                var text = reader.ReadToEnd();
                return text.Split("\n---\n");
            },
        };

        using var reader = new StringReader(input);
        var blocks = BlockParser.ParseBlocks(reader, config).ToList();

        Assert.Equal(2, blocks.Count);
        // Each block starts with "key1" which becomes Name via default handler
        Assert.Equal("key1", blocks[0][0].Value);
        Assert.Equal("key1", blocks[1][0].Value);
    }

    [Fact]
    public void ParseBlocksOptionsHandlerFactory()
    {
        var input = "iface0\n  flags 1234<UP,RUNNING,MULTICAST>";

        var config = new ParserConfig
        {
            Tokenizer = new TokenizerConfig { Separators = ['='], StripTrailing = [':'], EmitNewLineTokens = true },
            Keywords = new()
            {
                { "flags", new KeywordDef { CustomHandler = KeywordHandlers.Options('<', '>') } },
            },
            FirstTokenHandler = static (enumerator, _) => [new Pair("Name", enumerator.Current, [])],
        };

        using var reader = new StringReader(input);
        var blocks = BlockParser.ParseBlocks(reader, config).ToList();

        Assert.Single(blocks);
        Assert.Equal("iface0", blocks[0][0].Value);

        var flagsPair = blocks[0].First(p => p.Key == "flags");
        Assert.Equal(ChildType.ObjectType, flagsPair.ChildType);

        var bits = flagsPair.Children.First(c => c.Key == "Bits");
        Assert.Equal("1234", bits.Value);

        var values = flagsPair.Children.First(c => c.Key == "Values");
        Assert.Equal(ChildType.ArrayType, values.ChildType);
        Assert.Equal(3, values.Children.Count);
        Assert.Equal("UP", values.Children[0].Value);
        Assert.Equal("RUNNING", values.Children[1].Value);
        Assert.Equal("MULTICAST", values.Children[2].Value);
    }

    [Fact]
    public void ParseBlocksNewLineKind()
    {
        var input = "block1\n  media autoselect (1000baseT full-duplex)";

        var config = new ParserConfig
        {
            Tokenizer = new TokenizerConfig { Separators = [], EmitNewLineTokens = true },
            Keywords = new()
            {
                { "media", new KeywordDef { Kind = KeywordKind.NewLine } },
            },
        };

        using var reader = new StringReader(input);
        var blocks = BlockParser.ParseBlocks(reader, config).ToList();

        Assert.Single(blocks);
        var mediaPair = blocks[0].First(p => p.Key == "media");
        Assert.Equal("autoselect (1000baseT full-duplex)", mediaPair.Value);
    }

    [Fact]
    public void ParseBlocksSingleKind()
    {
        var input = "block1\n  secured";

        var config = new ParserConfig
        {
            Tokenizer = new TokenizerConfig { Separators = [], EmitNewLineTokens = true },
            Keywords = new()
            {
                { "secured", new KeywordDef { Kind = KeywordKind.Single } },
            },
        };

        using var reader = new StringReader(input);
        var blocks = BlockParser.ParseBlocks(reader, config).ToList();

        Assert.Single(blocks);
        var securedPair = blocks[0].First(p => p.Key == "secured");
        Assert.Equal("true", securedPair.Value);
    }

    [Fact]
    public void ParseBlocksGroupKeyword()
    {
        var input = "block1\n  group\n    child1 v1\n    child2 v2";

        var config = new ParserConfig
        {
            Tokenizer = new TokenizerConfig { Separators = [], EmitNewLineTokens = true },
            Keywords = new()
            {
                {
                    "group", new KeywordDef
                    {
                        Kind = KeywordKind.Group,
                        ChildKeywords = new()
                        {
                            { "child1", new KeywordDef { Kind = KeywordKind.Next } },
                            { "child2", new KeywordDef { Kind = KeywordKind.Next } },
                        },
                    }
                },
            },
        };

        using var reader = new StringReader(input);
        var blocks = BlockParser.ParseBlocks(reader, config).ToList();

        Assert.Single(blocks);
        var groupPair = blocks[0].First(p => p.Key == "group");
        Assert.Equal(ChildType.ObjectType, groupPair.ChildType);
        Assert.Equal(2, groupPair.Children.Count);
        Assert.Equal("child1", groupPair.Children[0].Key);
        Assert.Equal("v1", groupPair.Children[0].Value);
        Assert.Equal("child2", groupPair.Children[1].Key);
        Assert.Equal("v2", groupPair.Children[1].Value);
    }

    [Fact]
    public void ParseBlocksChildCustomHandler()
    {
        var input = "block1\n  group\n    flags 0xaa<A,B>";

        var config = new ParserConfig
        {
            Tokenizer = new TokenizerConfig { Separators = [], EmitNewLineTokens = true },
            Keywords = new()
            {
                {
                    "group", new KeywordDef
                    {
                        Kind = KeywordKind.Group,
                        ChildKeywords = new()
                        {
                            { "flags", new KeywordDef { CustomHandler = KeywordHandlers.Options('<', '>') } },
                        },
                    }
                },
            },
        };

        using var reader = new StringReader(input);
        var blocks = BlockParser.ParseBlocks(reader, config).ToList();

        Assert.Single(blocks);
        var groupPair = blocks[0].First(p => p.Key == "group");
        var flagsPair = groupPair.Children.First(c => c.Key == "flags");
        Assert.Equal(ChildType.ObjectType, flagsPair.ChildType);

        var bits = flagsPair.Children.First(c => c.Key == "Bits");
        Assert.Equal("0xaa", bits.Value);
    }

    [Fact]
    public void PairWriterKeyTransformOverride()
    {
        var pairs = new List<IReadOnlyList<Pair>>
        {
            new List<Pair> { new("some_key", "value", []) },
        };

        using var sw = new StringWriter();
        PairWriter.WriteKeyValues(pairs, sw, keyTransform: key => key.ToUpperInvariant());

        var output = sw.ToString();
        Assert.Contains("SOME_KEY: value", output);
    }

    #endregion

    #region TokenizeWithDepth Tests

    [Fact]
    public void TokenizeWithDepthBasicDepths()
    {
        var config = new TokenizerConfig { Separators = [], MeasureIndentation = true };
        using var reader = new StringReader("root val\n  child val2\n    deep val3");
        var tokens = BlockParser.TokenizeWithDepth(reader, config).ToList();

        // root line: depth 0
        Assert.Equal(new Token("root", 0), tokens[0]);
        Assert.Equal(new Token("val", 0), tokens[1]);
        Assert.Equal(new Token(Helpers.NewLine, 0), tokens[2]);

        // child line: depth 2
        Assert.Equal(new Token("child", 2), tokens[3]);
        Assert.Equal(new Token("val2", 2), tokens[4]);
        Assert.Equal(new Token(Helpers.NewLine, 0), tokens[5]);

        // deep line: depth 4
        Assert.Equal(new Token("deep", 4), tokens[6]);
        Assert.Equal(new Token("val3", 4), tokens[7]);
    }

    [Fact]
    public void TokenizeWithDepthSeparatorsPreserved()
    {
        var config = new TokenizerConfig { Separators = [':'], MeasureIndentation = true };
        using var reader = new StringReader("key:value\n  child:cval");
        var tokens = BlockParser.TokenizeWithDepth(reader, config)
            .Where(t => t.Value != Helpers.NewLine)
            .ToList();

        Assert.Equal(new Token("key", 0), tokens[0]);
        Assert.Equal(new Token("value", 0), tokens[1]);
        Assert.Equal(new Token("child", 2), tokens[2]);
        Assert.Equal(new Token("cval", 2), tokens[3]);
    }

    [Fact]
    public void TokenizeWithDepthEmptyInput()
    {
        var config = new TokenizerConfig { Separators = [], MeasureIndentation = true };
        using var reader = new StringReader("");
        var tokens = BlockParser.TokenizeWithDepth(reader, config).ToList();
        Assert.Empty(tokens);
    }

    #endregion

    #region Indentation-Aware ParseBlock Tests

    [Fact]
    public void ParseBlockIndentationTwoLevels()
    {
        var input = "name value1\n  child1 value2\n  child2 value3";

        var config = new ParserConfig
        {
            Tokenizer = new TokenizerConfig { Separators = [], MeasureIndentation = true, EmitNewLineTokens = true },
            Keywords = new(),
        };

        var result = BlockParser.ParseBlock(input, config);

        Assert.Single(result);
        var root = result[0];
        Assert.Equal("name", root.Key);
        Assert.Equal("value1", root.Value);
        Assert.Equal(ChildType.ObjectType, root.ChildType);
        Assert.Equal(2, root.Children.Count);
        Assert.Equal("child1", root.Children[0].Key);
        Assert.Equal("value2", root.Children[0].Value);
        Assert.Equal("child2", root.Children[1].Key);
        Assert.Equal("value3", root.Children[1].Value);
    }

    [Fact]
    public void ParseBlockIndentationThreeLevels()
    {
        var input = "root\n  mid value\n    deep value2";

        var config = new ParserConfig
        {
            Tokenizer = new TokenizerConfig { Separators = [], MeasureIndentation = true, EmitNewLineTokens = true },
            Keywords = new(),
        };

        var result = BlockParser.ParseBlock(input, config);

        Assert.Single(result);
        var root = result[0];
        Assert.Equal("root", root.Key);
        Assert.Empty(root.Value);
        Assert.Single(root.Children);

        var mid = root.Children[0];
        Assert.Equal("mid", mid.Key);
        Assert.Equal("value", mid.Value);
        Assert.Single(mid.Children);

        var deep = mid.Children[0];
        Assert.Equal("deep", deep.Key);
        Assert.Equal("value2", deep.Value);
    }

    [Fact]
    public void ParseBlockIndentationMultipleToplevel()
    {
        var input = "entry1 val1\n  child1 val2\nentry2 val3\n  child2 val4";

        var config = new ParserConfig
        {
            Tokenizer = new TokenizerConfig { Separators = [], MeasureIndentation = true, EmitNewLineTokens = true },
            Keywords = new(),
        };

        var result = BlockParser.ParseBlock(input, config);

        Assert.Equal(2, result.Count);
        Assert.Equal("entry1", result[0].Key);
        Assert.Equal("val1", result[0].Value);
        Assert.Single(result[0].Children);
        Assert.Equal("child1", result[0].Children[0].Key);

        Assert.Equal("entry2", result[1].Key);
        Assert.Equal("val3", result[1].Value);
        Assert.Single(result[1].Children);
        Assert.Equal("child2", result[1].Children[0].Key);
    }

    [Fact]
    public void ParseBlockIndentationEmptyInput()
    {
        var config = new ParserConfig
        {
            Tokenizer = new TokenizerConfig { Separators = [], MeasureIndentation = true, EmitNewLineTokens = true },
            Keywords = new(),
        };

        var result = BlockParser.ParseBlock("", config);
        Assert.Empty(result);
    }

    [Fact]
    public void ParseBlockIndentationSiblingsThenReturn()
    {
        // Tests depth returning to a shallower level after deep nesting.
        var input = "root\n  mid1\n    deep1 val\n  mid2\n    deep2 val";

        var config = new ParserConfig
        {
            Tokenizer = new TokenizerConfig { Separators = [], MeasureIndentation = true, EmitNewLineTokens = true },
            Keywords = new(),
        };

        var result = BlockParser.ParseBlock(input, config);

        Assert.Single(result);
        var root = result[0];
        Assert.Equal(2, root.Children.Count);

        Assert.Equal("mid1", root.Children[0].Key);
        Assert.Single(root.Children[0].Children);
        Assert.Equal("deep1", root.Children[0].Children[0].Key);
        Assert.Equal("val", root.Children[0].Children[0].Value);

        Assert.Equal("mid2", root.Children[1].Key);
        Assert.Single(root.Children[1].Children);
        Assert.Equal("deep2", root.Children[1].Children[0].Key);
        Assert.Equal("val", root.Children[1].Children[0].Value);
    }

    [Fact]
    public void ParseBlockIndentationKeyOnly()
    {
        // A line with just a key and no value, followed by children.
        var input = "parent\n  key1 val1\n  key2 val2";

        var config = new ParserConfig
        {
            Tokenizer = new TokenizerConfig { Separators = [], MeasureIndentation = true, EmitNewLineTokens = true },
            Keywords = new(),
        };

        var result = BlockParser.ParseBlock(input, config);

        Assert.Single(result);
        Assert.Equal("parent", result[0].Key);
        Assert.Empty(result[0].Value);
        Assert.Equal(2, result[0].Children.Count);
    }

    [Fact]
    public void ParseBlockIndentationMultiWordValue()
    {
        // Multiple tokens after the key on the same line are joined as the value.
        var input = "media autoselect 1000baseT full-duplex";

        var config = new ParserConfig
        {
            Tokenizer = new TokenizerConfig { Separators = [], MeasureIndentation = true, EmitNewLineTokens = true },
            Keywords = new(),
        };

        var result = BlockParser.ParseBlock(input, config);

        Assert.Single(result);
        Assert.Equal("media", result[0].Key);
        Assert.Equal("autoselect 1000baseT full-duplex", result[0].Value);
    }

    [Fact]
    public void ParseBlocksIndentationEndToEnd()
    {
        // End-to-end through ParseBlocks with block splitting.
        var input = "en0\n  inet 192.168.1.1\n  netmask 255.255.255.0\nen1\n  inet 10.0.0.1";

        var config = new ParserConfig
        {
            Tokenizer = new TokenizerConfig { Separators = [], MeasureIndentation = true, EmitNewLineTokens = true },
            Keywords = new(),
        };

        using var reader = new StringReader(input);
        var blocks = BlockParser.ParseBlocks(reader, config).ToList();

        Assert.Equal(2, blocks.Count);

        Assert.Equal("en0", blocks[0][0].Key);
        Assert.Equal(2, blocks[0][0].Children.Count);
        Assert.Equal("inet", blocks[0][0].Children[0].Key);
        Assert.Equal("192.168.1.1", blocks[0][0].Children[0].Value);

        Assert.Equal("en1", blocks[1][0].Key);
        Assert.Single(blocks[1][0].Children);
        Assert.Equal("inet", blocks[1][0].Children[0].Key);
        Assert.Equal("10.0.0.1", blocks[1][0].Children[0].Value);
    }

    #endregion
}
