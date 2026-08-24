using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.does_not_equals;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.does_not_equals") { }

    [Fact(DisplayName = "S11.9.2_A7.8")]
    public Task S11_9_2_A7_8()
        => ExecutionTest("S11.9.2_A7.8");

    [Fact(DisplayName = "S11.9.2_A7.9")]
    public Task S11_9_2_A7_9()
        => ExecutionTest("S11.9.2_A7.9");
}
