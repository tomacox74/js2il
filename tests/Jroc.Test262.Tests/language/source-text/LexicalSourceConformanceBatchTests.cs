using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.source_text;

public class LexicalSourceConformanceBatchTests : FileSystemExecutionTestsBase
{
    public LexicalSourceConformanceBatchTests() : base("language/source-text", "language.source_text") { }

    [Fact(DisplayName = "6.1.js")]
    public Task _6_1()
        => ExecutionTest("6.1");
}
