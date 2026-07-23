using Jroc.Test262.Tests.built_ins;



namespace Jroc.Test262.Tests.built_ins.String.prototype.indexOf;



public class ExecutionTests : DiskExecutionTestsBase

{

    public ExecutionTests() : base("built_ins.String.prototype.indexOf") { }



    [Fact(DisplayName = "S15.5.4.7_A10")]
    public Task S15_5_4_7_A10()

        => ExecutionTestFromFile("S15.5.4.7_A10");



    [Fact(DisplayName = "S15.5.4.7_A11")]
    public Task S15_5_4_7_A11()

        => ExecutionTestFromFile("S15.5.4.7_A11");



    [Fact(DisplayName = "S15.5.4.7_A1_T1")]
    public Task S15_5_4_7_A1_T1()

        => ExecutionTestFromFile("S15.5.4.7_A1_T1");



    [Fact(DisplayName = "S15.5.4.7_A1_T10")]

    public Task S15_5_4_7_A1_T10()

        => ExecutionTestFromFile("S15.5.4.7_A1_T10");



    [Fact(DisplayName = "S15.5.4.7_A1_T12")]

    public Task S15_5_4_7_A1_T12()

        => ExecutionTestFromFile("S15.5.4.7_A1_T12");



    [Fact(DisplayName = "S15.5.4.7_A1_T2")]

    public Task S15_5_4_7_A1_T2()

        => ExecutionTestFromFile("S15.5.4.7_A1_T2");



    [Fact(DisplayName = "S15.5.4.7_A1_T4")]
    public Task S15_5_4_7_A1_T4()

        => ExecutionTestFromFile("S15.5.4.7_A1_T4");



    [Fact(DisplayName = "S15.5.4.7_A1_T5")]

    public Task S15_5_4_7_A1_T5()

        => ExecutionTestFromFile("S15.5.4.7_A1_T5");



    [Fact(DisplayName = "S15.5.4.7_A1_T6")]

    public Task S15_5_4_7_A1_T6()

        => ExecutionTestFromFile("S15.5.4.7_A1_T6");



    [Fact(DisplayName = "S15.5.4.7_A1_T7")]

    public Task S15_5_4_7_A1_T7()

        => ExecutionTestFromFile("S15.5.4.7_A1_T7");



    [Fact(DisplayName = "S15.5.4.7_A1_T8")]
    public Task S15_5_4_7_A1_T8()

        => ExecutionTestFromFile("S15.5.4.7_A1_T8");



    [Fact(DisplayName = "S15.5.4.7_A1_T9")]
    public Task S15_5_4_7_A1_T9()

        => ExecutionTestFromFile("S15.5.4.7_A1_T9");



    [Fact(DisplayName = "S15.5.4.7_A2_T1")]
    public Task S15_5_4_7_A2_T1()

        => ExecutionTestFromFile("S15.5.4.7_A2_T1");



    [Fact(DisplayName = "S15.5.4.7_A2_T2")]
    public Task S15_5_4_7_A2_T2()

        => ExecutionTestFromFile("S15.5.4.7_A2_T2");



    [Fact(DisplayName = "S15.5.4.7_A2_T3")]
    public Task S15_5_4_7_A2_T3()

        => ExecutionTestFromFile("S15.5.4.7_A2_T3");



    [Fact(DisplayName = "S15.5.4.7_A2_T4")]
    public Task S15_5_4_7_A2_T4()

        => ExecutionTestFromFile("S15.5.4.7_A2_T4");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "position-tointeger")]
    public Task position_tointeger()
        => ExecutionTestFromFile("position-tointeger");

    [Fact(DisplayName = "S15.5.4.7_A3_T1")]
    public Task S15_5_4_7_A3_T1()
        => ExecutionTestFromFile("S15.5.4.7_A3_T1");

    [Fact(DisplayName = "S15.5.4.7_A3_T2", Skip = "Blocked: eval is not supported yet.")]
    public Task S15_5_4_7_A3_T2()
        => ExecutionTestFromFile("S15.5.4.7_A3_T2");

    [Fact(DisplayName = "not-a-constructor")] public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
    [Fact(DisplayName = "position-tointeger-bigint")] public Task position_tointeger_bigint() => ExecutionTestFromFile("position-tointeger-bigint");
    [Fact(DisplayName = "position-tointeger-errors")] public Task position_tointeger_errors() => ExecutionTestFromFile("position-tointeger-errors");
    [Fact(DisplayName = "position-tointeger-toprimitive")] public Task position_tointeger_toprimitive() => ExecutionTestFromFile("position-tointeger-toprimitive");
    [Fact(DisplayName = "position-tointeger-wrapped-values")] public Task position_tointeger_wrapped_values() => ExecutionTestFromFile("position-tointeger-wrapped-values");
    [Fact(DisplayName = "S15.5.4.7_A3_T3")] public Task S15_5_4_7_A3_T3() => ExecutionTestFromFile("S15.5.4.7_A3_T3");
    [Fact(DisplayName = "S15.5.4.7_A4_T1")] public Task S15_5_4_7_A4_T1() => ExecutionTestFromFile("S15.5.4.7_A4_T1");
    [Fact(DisplayName = "S15.5.4.7_A4_T2")] public Task S15_5_4_7_A4_T2() => ExecutionTestFromFile("S15.5.4.7_A4_T2");
    [Fact(DisplayName = "S15.5.4.7_A4_T3")] public Task S15_5_4_7_A4_T3() => ExecutionTestFromFile("S15.5.4.7_A4_T3");
    [Fact(DisplayName = "S15.5.4.7_A4_T4")] public Task S15_5_4_7_A4_T4() => ExecutionTestFromFile("S15.5.4.7_A4_T4");
    [Fact(DisplayName = "S15.5.4.7_A4_T5")] public Task S15_5_4_7_A4_T5() => ExecutionTestFromFile("S15.5.4.7_A4_T5");
    [Fact(DisplayName = "S15.5.4.7_A5_T1")] public Task S15_5_4_7_A5_T1() => ExecutionTestFromFile("S15.5.4.7_A5_T1");
    [Fact(DisplayName = "S15.5.4.7_A5_T2")] public Task S15_5_4_7_A5_T2() => ExecutionTestFromFile("S15.5.4.7_A5_T2");
    [Fact(DisplayName = "S15.5.4.7_A5_T3")] public Task S15_5_4_7_A5_T3() => ExecutionTestFromFile("S15.5.4.7_A5_T3");
    [Fact(DisplayName = "S15.5.4.7_A5_T4")] public Task S15_5_4_7_A5_T4() => ExecutionTestFromFile("S15.5.4.7_A5_T4");
    [Fact(DisplayName = "S15.5.4.7_A5_T5")] public Task S15_5_4_7_A5_T5() => ExecutionTestFromFile("S15.5.4.7_A5_T5");
    [Fact(DisplayName = "S15.5.4.7_A5_T6")] public Task S15_5_4_7_A5_T6() => ExecutionTestFromFile("S15.5.4.7_A5_T6");
    [Fact(DisplayName = "S15.5.4.7_A6")] public Task S15_5_4_7_A6() => ExecutionTestFromFile("S15.5.4.7_A6");
    [Fact(DisplayName = "S15.5.4.7_A7")] public Task S15_5_4_7_A7() => ExecutionTestFromFile("S15.5.4.7_A7");
    [Fact(DisplayName = "S15.5.4.7_A8")] public Task S15_5_4_7_A8() => ExecutionTestFromFile("S15.5.4.7_A8");
    [Fact(DisplayName = "S15.5.4.7_A9")] public Task S15_5_4_7_A9() => ExecutionTestFromFile("S15.5.4.7_A9");
    [Fact(DisplayName = "searchstring-tostring-bigint")] public Task searchstring_tostring_bigint() => ExecutionTestFromFile("searchstring-tostring-bigint");
    [Fact(DisplayName = "searchstring-tostring-errors")] public Task searchstring_tostring_errors() => ExecutionTestFromFile("searchstring-tostring-errors");
    [Fact(DisplayName = "searchstring-tostring-toprimitive")] public Task searchstring_tostring_toprimitive() => ExecutionTestFromFile("searchstring-tostring-toprimitive");
    [Fact(DisplayName = "searchstring-tostring-wrapped-values")] public Task searchstring_tostring_wrapped_values() => ExecutionTestFromFile("searchstring-tostring-wrapped-values");
    [Fact(DisplayName = "searchstring-tostring")] public Task searchstring_tostring() => ExecutionTestFromFile("searchstring-tostring");
    [Fact(DisplayName = "this-value-not-obj-coercible")] public Task this_value_not_obj_coercible() => ExecutionTestFromFile("this-value-not-obj-coercible");
}
