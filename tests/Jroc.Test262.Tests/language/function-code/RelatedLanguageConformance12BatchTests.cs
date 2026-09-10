using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.function_code;

public sealed class RelatedLanguageConformance12BatchTests : DiskExecutionTestsBase
{
    public RelatedLanguageConformance12BatchTests() : base("language.function_code") { }

    [Fact(DisplayName = "language/function-code/10.4.3-1-100-s.js")]
    public Task test_10_4_3_1_100_s()
        => ExecutionTest("10.4.3-1-100-s");

    [Fact(DisplayName = "language/function-code/10.4.3-1-100gs.js")]
    public Task test_10_4_3_1_100gs()
        => ExecutionTest("10.4.3-1-100gs");

    [Fact(DisplayName = "language/function-code/10.4.3-1-101-s.js")]
    public Task test_10_4_3_1_101_s()
        => ExecutionTest("10.4.3-1-101-s");

    [Fact(DisplayName = "language/function-code/10.4.3-1-101gs.js")]
    public Task test_10_4_3_1_101gs()
        => ExecutionTest("10.4.3-1-101gs");
}
