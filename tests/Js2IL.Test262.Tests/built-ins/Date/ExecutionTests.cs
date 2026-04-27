using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Date;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Date") { }

    [Fact(DisplayName = "15.9.1.15-1", Skip = "Known JS2IL defect")]
    public Task _15_9_1_15_1()
        => ExecutionTest("15.9.1.15-1");

    [Fact(DisplayName = "coercion-order", Skip = "Known JS2IL defect")]
    public Task coercion_order()
        => ExecutionTest("coercion-order");
}
