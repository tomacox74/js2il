using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Array;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array") { }

    [Fact(DisplayName = "15.4.5-1")]
    public Task _15_4_5_1()
        => ExecutionTest("15.4.5-1");

    [Fact(DisplayName = "15.4.5.1-5-1")]
    public Task _15_4_5_1_5_1()
        => ExecutionTest("15.4.5.1-5-1");

    [Fact(DisplayName = "15.4.5.1-5-2")]
    public Task _15_4_5_1_5_2()
        => ExecutionTest("15.4.5.1-5-2");

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTest("constructor", preferOutOfProc: true);
    [Fact(DisplayName = "property-cast-boolean-primitive")]
    public Task property_cast_boolean_primitive()
        => ExecutionTest("property-cast-boolean-primitive");
    [Fact(DisplayName = "property-cast-number")]
    public Task property_cast_number()
        => ExecutionTest("property-cast-number");
    [Fact(DisplayName = "property-cast-nan-infinity")]
    public Task property_cast_nan_infinity()
        => ExecutionTest("property-cast-nan-infinity");
    [Fact(DisplayName = "S15.4.5.2_A1_T1")]
    public Task S15_4_5_2_A1_T1()
        => ExecutionTest("S15.4.5.2_A1_T1", preferOutOfProc: true);

}
