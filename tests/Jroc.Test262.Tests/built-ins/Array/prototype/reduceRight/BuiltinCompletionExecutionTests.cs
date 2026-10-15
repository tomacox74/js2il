using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.reduceRight;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.Array.prototype.reduceRight") { }

    [Fact(DisplayName = "15.4.4.22-9-4.js")]
    public Task test_15_4_4_22_9_4() => ExecutionTest("15.4.4.22-9-4");

    [Fact(DisplayName = "15.4.4.22-9-9.js")]
    public Task test_15_4_4_22_9_9() => ExecutionTest("15.4.4.22-9-9");

    [Fact(DisplayName = "15.4.4.22-9-b-22.js")]
    public Task test_15_4_4_22_9_b_22() => ExecutionTest("15.4.4.22-9-b-22");

    [Fact(DisplayName = "15.4.4.22-9-b-24.js")]
    public Task test_15_4_4_22_9_b_24() => ExecutionTest("15.4.4.22-9-b-24");

    [Fact(DisplayName = "15.4.4.22-9-b-27.js")]
    public Task test_15_4_4_22_9_b_27() => ExecutionTest("15.4.4.22-9-b-27");

    [Fact(DisplayName = "15.4.4.22-9-b-28.js")]
    public Task test_15_4_4_22_9_b_28() => ExecutionTest("15.4.4.22-9-b-28");

    [Fact(DisplayName = "15.4.4.22-9-b-9.js")]
    public Task test_15_4_4_22_9_b_9() => ExecutionTest("15.4.4.22-9-b-9");

    [Fact(DisplayName = "15.4.4.22-9-c-1.js")]
    public Task test_15_4_4_22_9_c_1() => ExecutionTest("15.4.4.22-9-c-1");

    [Fact(DisplayName = "length-near-integer-limit.js")]
    public Task test_length_near_integer_limit() => ExecutionTest("length-near-integer-limit");
}
