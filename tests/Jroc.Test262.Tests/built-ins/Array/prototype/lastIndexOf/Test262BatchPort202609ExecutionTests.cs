using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.lastIndexOf;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Array.prototype.lastIndexOf") { }

    [Fact(DisplayName = "15.4.4.15-8-b-1")]
    public Task _15_4_4_15_8_b_1()
        => ExecutionTestFromFile("15.4.4.15-8-b-1");

    [Fact(DisplayName = "15.4.4.15-8-b-ii-2")]
    public Task _15_4_4_15_8_b_ii_2()
        => ExecutionTestFromFile("15.4.4.15-8-b-ii-2");

    [Fact(DisplayName = "calls-only-has-on-prototype-after-length-zeroed")]
    public Task calls_only_has_on_prototype_after_length_zeroed()
        => ExecutionTestFromFile("calls-only-has-on-prototype-after-length-zeroed");

}
