using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.function_code;

public class LexicalSourceConformanceBatchTests : FileSystemExecutionTestsBase
{
    public LexicalSourceConformanceBatchTests() : base("language/function-code", "language.function_code") { }

    [Fact(DisplayName = "10.4.3-1-1-s.js")]
    public Task _10_4_3_1_1_s()
        => ExecutionTest("10.4.3-1-1-s");
}
