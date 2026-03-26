namespace JMW.Parsing.Tests;

public class HelpersTests
{
    [Fact]
    public void GetBlocksDefaultTest()
    {
        const string input = @"
block1 line1
  block1 line2

block2 line1
  block2 line2
";

        using var reader = new StringReader(input);
        var blocks = Helpers.GetBlocks(reader).ToList();

        Assert.Equal(2, blocks.Count);
        Assert.StartsWith("block1", blocks[0]);
        Assert.StartsWith("block2", blocks[1]);
    }

    [Fact]
    public void GetBlocksTrimEndingWhitespaceTest()
    {
        const string input = @"block1 line1
  block1 line2   

block2 line1
  block2 line2  
";

        using var reader = new StringReader(input);
        var blocks = Helpers.GetBlocks(reader, trimInitialWhitespace: false, trimEndingWhitespace: true).ToList();

        Assert.Equal(2, blocks.Count);
        // Trailing whitespace should be trimmed from each block
        Assert.False(char.IsWhiteSpace(blocks[0][^1]));
        Assert.False(char.IsWhiteSpace(blocks[1][^1]));
    }

    [Fact]
    public void GetBlocksNoTrimInitialWhitespaceTest()
    {
        const string input = @"  block1 line1

block2 line1
";

        using var reader = new StringReader(input);
        var blocks = Helpers.GetBlocks(reader, trimInitialWhitespace: false).ToList();

        // Without trimming, initial whitespace is preserved in the first block
        Assert.True(char.IsWhiteSpace(blocks[0][0]));
    }

    [Fact]
    public void GetBlocksEmptyInputTest()
    {
        using var reader = new StringReader("");
        var blocks = Helpers.GetBlocks(reader).ToList();

        Assert.Empty(blocks);
    }

    [Fact]
    public void GetBlocksSingleBlockNoTrailingNewlineTest()
    {
        const string input = "block1 line1\n  block1 line2";

        using var reader = new StringReader(input);
        var blocks = Helpers.GetBlocks(reader).ToList();

        Assert.Single(blocks);
        Assert.StartsWith("block1", blocks[0]);
    }
}
