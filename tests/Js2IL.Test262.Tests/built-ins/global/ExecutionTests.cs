using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.global_;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.global") { }

    [Fact(DisplayName = "10.2.1.1.3-4-16-s", Skip = "Blocked: strict assignment to global NaN does not throw yet.")]
    public Task _10_2_1_1_3_4_16_s()
        => ExecutionTestFromFile("10.2.1.1.3-4-16-s");

    [Fact(DisplayName = "10.2.1.1.3-4-18-s", Skip = "Blocked: strict assignment to global undefined does not throw yet.")]
    public Task _10_2_1_1_3_4_18_s()
        => ExecutionTestFromFile("10.2.1.1.3-4-18-s");

    [Fact(DisplayName = "10.2.1.1.3-4-22")]
    public Task _10_2_1_1_3_4_22()
        => ExecutionTestFromFile("10.2.1.1.3-4-22");

    [Fact(DisplayName = "10.2.1.1.3-4-27")]
    public Task _10_2_1_1_3_4_27()
        => ExecutionTestFromFile("10.2.1.1.3-4-27");

    [Fact(DisplayName = "S10.2.3_A1.1_T1")]
    public Task S10_2_3_A1_1_T1()
        => ExecutionTestFromFile("S10.2.3_A1.1_T1");

    [Fact(DisplayName = "S10.2.3_A1.1_T2", Skip = "Blocked: eval is not supported yet.")]
    public Task S10_2_3_A1_1_T2()
        => ExecutionTestFromFile("S10.2.3_A1.1_T2");

    [Fact(DisplayName = "S10.2.3_A1.1_T3")]
    public Task S10_2_3_A1_1_T3()
        => ExecutionTestFromFile("S10.2.3_A1.1_T3");

    [Fact(DisplayName = "S10.2.3_A1.1_T4")]
    public Task S10_2_3_A1_1_T4()
        => ExecutionTestFromFile("S10.2.3_A1.1_T4");

    [Fact(DisplayName = "S10.2.3_A1.2_T1")]
    public Task S10_2_3_A1_2_T1()
        => ExecutionTestFromFile("S10.2.3_A1.2_T1");

    [Fact(DisplayName = "S10.2.3_A1.2_T2", Skip = "Blocked: eval is not supported yet.")]
    public Task S10_2_3_A1_2_T2()
        => ExecutionTestFromFile("S10.2.3_A1.2_T2");

    [Fact(DisplayName = "S10.2.3_A1.2_T3")]
    public Task S10_2_3_A1_2_T3()
        => ExecutionTestFromFile("S10.2.3_A1.2_T3");

    [Fact(DisplayName = "S10.2.3_A1.2_T4")]
    public Task S10_2_3_A1_2_T4()
        => ExecutionTestFromFile("S10.2.3_A1.2_T4");

    [Fact(DisplayName = "S10.2.3_A1.3_T1", Skip = "Blocked: eval is not supported yet.")]
    public Task S10_2_3_A1_3_T1()
        => ExecutionTestFromFile("S10.2.3_A1.3_T1");

    [Fact(DisplayName = "S10.2.3_A1.3_T2", Skip = "Blocked: eval is not supported yet.")]
    public Task S10_2_3_A1_3_T2()
        => ExecutionTestFromFile("S10.2.3_A1.3_T2");

    [Fact(DisplayName = "S10.2.3_A1.3_T3", Skip = "Blocked: eval is not supported yet.")]
    public Task S10_2_3_A1_3_T3()
        => ExecutionTestFromFile("S10.2.3_A1.3_T3");

    [Fact(DisplayName = "S10.2.3_A1.3_T4", Skip = "Blocked: eval is not supported yet.")]
    public Task S10_2_3_A1_3_T4()
        => ExecutionTestFromFile("S10.2.3_A1.3_T4");

    [Fact(DisplayName = "S10.2.3_A2.1_T1")]
    public Task S10_2_3_A2_1_T1()
        => ExecutionTestFromFile("S10.2.3_A2.1_T1");

    [Fact(DisplayName = "S10.2.3_A2.1_T2")]
    public Task S10_2_3_A2_1_T2()
        => ExecutionTestFromFile("S10.2.3_A2.1_T2");

    [Fact(DisplayName = "S10.2.3_A2.1_T3")]
    public Task S10_2_3_A2_1_T3()
        => ExecutionTestFromFile("S10.2.3_A2.1_T3");

    [Fact(DisplayName = "S10.2.3_A2.1_T4")]
    public Task S10_2_3_A2_1_T4()
        => ExecutionTestFromFile("S10.2.3_A2.1_T4");

    [Fact(DisplayName = "S10.2.3_A2.3_T1", Skip = "Blocked: eval is not supported yet.")]
    public Task S10_2_3_A2_3_T1()
        => ExecutionTestFromFile("S10.2.3_A2.3_T1");

    [Fact(DisplayName = "S10.2.3_A2.3_T2", Skip = "Blocked: eval is not supported yet.")]
    public Task S10_2_3_A2_3_T2()
        => ExecutionTestFromFile("S10.2.3_A2.3_T2");

    [Fact(DisplayName = "S10.2.3_A2.3_T3", Skip = "Blocked: eval is not supported yet.")]
    public Task S10_2_3_A2_3_T3()
        => ExecutionTestFromFile("S10.2.3_A2.3_T3");

    [Fact(DisplayName = "S10.2.3_A2.3_T4", Skip = "Blocked: eval is not supported yet.")]
    public Task S10_2_3_A2_3_T4()
        => ExecutionTestFromFile("S10.2.3_A2.3_T4");

    [Fact(DisplayName = "S15.1_A1_T1")]
    public Task S15_1_A1_T1()
        => ExecutionTestFromFile("S15.1_A1_T1");

    [Fact(DisplayName = "S15.1_A1_T2")]
    public Task S15_1_A1_T2()
        => ExecutionTestFromFile("S15.1_A1_T2");

    [Fact(DisplayName = "S15.1_A2_T1")]
    public Task S15_1_A2_T1()
        => ExecutionTestFromFile("S15.1_A2_T1");

    [Fact(DisplayName = "global-object", Skip = "Blocked: global object built-in identity coverage is incomplete.")]
    public Task global_object()
        => ExecutionTestFromFile("global-object");

    [Fact(DisplayName = "property-descriptor")]
    public Task property_descriptor()
        => ExecutionTestFromFile("property-descriptor");
}
