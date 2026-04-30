using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.RegExp;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExp") { }

    [Fact(DisplayName = "15.10.4.1-1")]
    public Task _15_10_4_1_1()
        => ExecutionTest("15.10.4.1-1");

    [Fact(DisplayName = "call_with_non_regexp_same_constructor")]
    public Task call_with_non_regexp_same_constructor()
        => ExecutionTest("call_with_non_regexp_same_constructor");
}
