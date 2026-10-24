namespace Jroc.Tests;

public sealed class GeneratorTestsBaseTests
{
    [Theory]
    [InlineData(@"D:\repo\tests\Jroc.Tests", @"D:\\repo\\tests\\Jroc.Tests\\Import\\entry.js")]
    [InlineData("/repo/tests/Jroc.Tests", "/repo/tests/Jroc.Tests/Import/entry.js")]
    public void NormalizeProjectSourcePaths_PreservesNonPathStringEscapes(string root, string source)
    {
        var il = "IL_0000: ldstr \"" + source + "\"\nIL_0005: ldstr \"\\\\w+\"";

        var normalized = GeneratorTestsBase.NormalizeProjectSourcePaths(il, root);

        Assert.Equal("IL_0000: ldstr \"{ProjectDirectory}Import/entry.js\"\nIL_0005: ldstr \"\\\\w+\"", normalized);
    }

    [Fact]
    public void RemoveVolatileMethodRvaComments_RemovesOnlyRvaCommentLines()
    {
        const string il =
            """
            .method public hidebysig
            {
                // Method begins at RVA 0x2284
                // Header size: 12
                IL_0000: ret
            }
            """;

        var filtered = GeneratorTestsBase.RemoveVolatileMethodRvaComments(il);

        Assert.Equal(
            """
            .method public hidebysig
            {
                // Header size: 12
                IL_0000: ret
            }
            """,
            filtered);
    }
}
