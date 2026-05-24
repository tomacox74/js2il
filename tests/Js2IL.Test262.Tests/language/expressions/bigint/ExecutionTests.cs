using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.bigint;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.bigint") { }

    [Fact(DisplayName = "bigint")]
    public Task unary_minus_bigint()
        => ExecutionTest("unary-minus/bigint");

    [Fact(DisplayName = "bigint")]
    public Task bitwise_not_bigint()
        => ExecutionTest("bitwise-not/bigint");

    [Fact(DisplayName = "bigint")]
    public Task left_shift_bigint()
        => ExecutionTest("left-shift/bigint");

    [Fact(DisplayName = "bigint")]
    public Task unsigned_right_shift_bigint()
        => ExecutionTest("unsigned-right-shift/bigint");
}
