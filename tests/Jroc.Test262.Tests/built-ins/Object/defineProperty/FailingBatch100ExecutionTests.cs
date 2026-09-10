using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.defineProperty;

public class FailingBatch100ExecutionTests : DiskExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Object.defineProperty") { }

    [Fact(DisplayName = "15.2.3.6-4-291-1")]
    public Task _15_2_3_6_4_291_1() => ExecutionTestFromFile("15.2.3.6-4-291-1");

    [Fact(DisplayName = "15.2.3.6-4-291")]
    public Task _15_2_3_6_4_291() => ExecutionTestFromFile("15.2.3.6-4-291");

    [Fact(DisplayName = "15.2.3.6-4-293-3")]
    public Task _15_2_3_6_4_293_3() => ExecutionTestFromFile("15.2.3.6-4-293-3");

    [Fact(DisplayName = "15.2.3.6-4-297-1")]
    public Task _15_2_3_6_4_297_1() => ExecutionTestFromFile("15.2.3.6-4-297-1");

    [Fact(DisplayName = "15.2.3.6-4-297")]
    public Task _15_2_3_6_4_297() => ExecutionTestFromFile("15.2.3.6-4-297");

    [Fact(DisplayName = "15.2.3.6-4-298-1")]
    public Task _15_2_3_6_4_298_1() => ExecutionTestFromFile("15.2.3.6-4-298-1");

    [Fact(DisplayName = "15.2.3.6-4-298")]
    public Task _15_2_3_6_4_298() => ExecutionTestFromFile("15.2.3.6-4-298");

    [Fact(DisplayName = "15.2.3.6-4-299-1")]
    public Task _15_2_3_6_4_299_1() => ExecutionTestFromFile("15.2.3.6-4-299-1");

    [Fact(DisplayName = "15.2.3.6-4-299")]
    public Task _15_2_3_6_4_299() => ExecutionTestFromFile("15.2.3.6-4-299");

    [Fact(DisplayName = "15.2.3.6-4-300")]
    public Task _15_2_3_6_4_300() => ExecutionTestFromFile("15.2.3.6-4-300");

    [Fact(DisplayName = "15.2.3.6-4-325-1")]
    public Task _15_2_3_6_4_325_1() => ExecutionTestFromFile("15.2.3.6-4-325-1");

    [Fact(DisplayName = "15.2.3.6-4-325")]
    public Task _15_2_3_6_4_325() => ExecutionTestFromFile("15.2.3.6-4-325");

    [Fact(DisplayName = "15.2.3.6-4-360-2")]
    public Task _15_2_3_6_4_360_2() => ExecutionTestFromFile("15.2.3.6-4-360-2");

    [Fact(DisplayName = "15.2.3.6-4-360-6")]
    public Task _15_2_3_6_4_360_6() => ExecutionTestFromFile("15.2.3.6-4-360-6");

}
