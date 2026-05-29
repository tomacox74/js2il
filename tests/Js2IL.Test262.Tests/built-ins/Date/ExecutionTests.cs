using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Date;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Date") { }

    [Fact(DisplayName = "15.9.1.15-1")]
    public Task _15_9_1_15_1()
        => ExecutionTestFromFile("15.9.1.15-1");

    [Fact(DisplayName = "coercion-order")]
    public Task coercion_order()
        => ExecutionTestFromFile("coercion-order");

    [Fact(DisplayName = "coercion-errors")]
    public Task coercion_errors()
        => ExecutionTestFromFile("coercion-errors");

    [Fact(DisplayName = "construct_with_date")]
    public Task construct_with_date()
        => ExecutionTestFromFile("construct_with_date");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "S15.9.2.1_A1")]
    public Task S15_9_2_1_A1()
        => ExecutionTestFromFile("S15.9.2.1_A1");

    [Fact(DisplayName = "S15.9.3.1_A1_T1")]
    public Task S15_9_3_1_A1_T1()
        => ExecutionTestFromFile("S15.9.3.1_A1_T1");

    [Fact(DisplayName = "TimeClip_negative_zero")]
    public Task TimeClip_negative_zero()
        => ExecutionTestFromFile("TimeClip_negative_zero");

    [Fact(DisplayName = "S15.9.3.1_A1_T2")]
    public Task S15_9_3_1_A1_T2()
        => ExecutionTestFromFile("S15.9.3.1_A1_T2");

    [Fact(DisplayName = "S15.9.3.1_A1_T3")]
    public Task S15_9_3_1_A1_T3()
        => ExecutionTestFromFile("S15.9.3.1_A1_T3");

    [Fact(DisplayName = "S15.9.3.1_A1_T4")]
    public Task S15_9_3_1_A1_T4()
        => ExecutionTestFromFile("S15.9.3.1_A1_T4");

    [Fact(DisplayName = "S15.9.3.1_A1_T5")]
    public Task S15_9_3_1_A1_T5()
        => ExecutionTestFromFile("S15.9.3.1_A1_T5");

    [Fact(DisplayName = "S15.9.3.1_A1_T6")]
    public Task S15_9_3_1_A1_T6()
        => ExecutionTestFromFile("S15.9.3.1_A1_T6");

}
