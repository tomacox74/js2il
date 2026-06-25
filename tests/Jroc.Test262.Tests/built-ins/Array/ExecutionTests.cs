using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array") { }

    [Fact(DisplayName = "15.4.5-1")]
    public Task _15_4_5_1()
        => ExecutionTestFromFile("15.4.5-1");

    [Fact(DisplayName = "15.4.5.1-5-1")]
    public Task _15_4_5_1_5_1()
        => ExecutionTestFromFile("15.4.5.1-5-1");

    [Fact(DisplayName = "15.4.5.1-5-2")]
    public Task _15_4_5_1_5_2()
        => ExecutionTestFromFile("15.4.5.1-5-2");

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "property-cast-boolean-primitive")]
    public Task property_cast_boolean_primitive()
        => ExecutionTestFromFile("property-cast-boolean-primitive");

    [Fact(DisplayName = "property-cast-number")]
    public Task property_cast_number()
        => ExecutionTestFromFile("property-cast-number");

    [Fact(DisplayName = "property-cast-nan-infinity")]
    public Task property_cast_nan_infinity()
        => ExecutionTestFromFile("property-cast-nan-infinity");

    [Fact(DisplayName = "S15.4.5.2_A1_T1")]
    public Task S15_4_5_2_A1_T1()
        => ExecutionTestFromFile("S15.4.5.2_A1_T1");

    [Fact(DisplayName = "S15.4.1_A1.1_T1")]
    public Task S15_4_1_A1_1_T1()
        => ExecutionTestFromFile("S15.4.1_A1.1_T1");

    [Fact(DisplayName = "S15.4.5.1_A2.3_T1")]
    public Task S15_4_5_1_A2_3_T1()
        => ExecutionTestFromFile("S15.4.5.1_A2.3_T1");

    [Fact(DisplayName = "S15.4.1_A1.1_T3")]
    public Task S15_4_1_A1_1_T3()
        => ExecutionTestFromFile("S15.4.1_A1.1_T3");

    [Fact(DisplayName = "S15.4.5.1_A2.1_T1")]
    public Task S15_4_5_1_A2_1_T1()
        => ExecutionTestFromFile("S15.4.5.1_A2.1_T1");

    [Fact(DisplayName = "S15.4.1_A1.3_T1")]
    public Task S15_4_1_A1_3_T1()
        => ExecutionTestFromFile("S15.4.1_A1.3_T1");

    [Fact(DisplayName = "S15.4.1_A2.1_T1")]
    public Task S15_4_1_A2_1_T1()
        => ExecutionTestFromFile("S15.4.1_A2.1_T1");

    [Fact(DisplayName = "S15.4.1_A2.2_T1")]
    public Task S15_4_1_A2_2_T1()
        => ExecutionTestFromFile("S15.4.1_A2.2_T1");

    [Fact(DisplayName = "S15.4.1_A3.1_T1")]
    public Task S15_4_1_A3_1_T1()
        => ExecutionTestFromFile("S15.4.1_A3.1_T1");

    [Fact(DisplayName = "S15.4.2.1_A1.1_T1")]
    public Task S15_4_2_1_A1_1_T1()
        => ExecutionTestFromFile("S15.4.2.1_A1.1_T1");

    [Fact(DisplayName = "S15.4.5.1_A2.2_T1")]
    public Task S15_4_5_1_A2_2_T1()
        => ExecutionTestFromFile("S15.4.5.1_A2.2_T1");

    [Fact(DisplayName = "S15.4.2.1_A1.1_T3")]
    public Task S15_4_2_1_A1_1_T3()
        => ExecutionTestFromFile("S15.4.2.1_A1.1_T3");

    [Fact(DisplayName = "S15.4.2.1_A2.1_T1")]
    public Task S15_4_2_1_A2_1_T1()
        => ExecutionTestFromFile("S15.4.2.1_A2.1_T1");

    [Fact(DisplayName = "S15.4.5.2_A1_T2")]
    public Task S15_4_5_2_A1_T2()
        => ExecutionTestFromFile("S15.4.5.2_A1_T2");

    [Fact(DisplayName = "S15.4.5.2_A2_T1")]
    public Task S15_4_5_2_A2_T1()
        => ExecutionTestFromFile("S15.4.5.2_A2_T1");

    [Fact(DisplayName = "S15.4.5.2_A3_T1")]
    public Task S15_4_5_2_A3_T1()
        => ExecutionTestFromFile("S15.4.5.2_A3_T1");

    [Fact(DisplayName = "S15.4.5.2_A3_T2")]
    public Task S15_4_5_2_A3_T2()
        => ExecutionTestFromFile("S15.4.5.2_A3_T2");
}
