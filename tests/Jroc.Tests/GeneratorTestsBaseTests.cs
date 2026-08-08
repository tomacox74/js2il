namespace Jroc.Tests;

public sealed class GeneratorTestsBaseTests
{
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
