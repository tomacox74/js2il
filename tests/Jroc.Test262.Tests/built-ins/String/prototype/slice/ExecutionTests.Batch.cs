namespace Jroc.Test262.Tests.built_ins.String.prototype.slice;

public partial class ExecutionTests
{
    [Fact(DisplayName = "S15.5.4.13_A2_T8.js")]
    public Task S15_5_4_13_A2_T8() => ExecutionTestFromFile("S15.5.4.13_A2_T8");
    [Fact(DisplayName = "S15.5.4.13_A2_T9.js")]
    public Task S15_5_4_13_A2_T9() => ExecutionTestFromFile("S15.5.4.13_A2_T9");
    [Fact(DisplayName = "S15.5.4.13_A3_T1.js")]
    public Task S15_5_4_13_A3_T1() => ExecutionTestFromFile("S15.5.4.13_A3_T1");
    [Fact(DisplayName = "S15.5.4.13_A3_T2.js")]
    public Task S15_5_4_13_A3_T2() => ExecutionTestFromFile("S15.5.4.13_A3_T2");
    [Fact(DisplayName = "S15.5.4.13_A3_T3.js")]
    public Task S15_5_4_13_A3_T3() => ExecutionTestFromFile("S15.5.4.13_A3_T3");
    [Fact(DisplayName = "S15.5.4.13_A3_T4.js")]
    public Task S15_5_4_13_A3_T4() => ExecutionTestFromFile("S15.5.4.13_A3_T4");
    [Fact(DisplayName = "S15.5.4.13_A6.js")]
    public Task S15_5_4_13_A6() => ExecutionTestFromFile("S15.5.4.13_A6");
    [Fact(DisplayName = "S15.5.4.13_A7.js")]
    public Task S15_5_4_13_A7() => ExecutionTestFromFile("S15.5.4.13_A7");
    [Fact(DisplayName = "S15.5.4.13_A8.js")]
    public Task S15_5_4_13_A8() => ExecutionTestFromFile("S15.5.4.13_A8");
    [Fact(DisplayName = "S15.5.4.13_A9.js")]
    public Task S15_5_4_13_A9() => ExecutionTestFromFile("S15.5.4.13_A9");
    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
    [Fact(DisplayName = "this-value-not-obj-coercible.js")]
    public Task this_value_not_obj_coercible() => ExecutionTestFromFile("this-value-not-obj-coercible");
    [Fact(DisplayName = "this-value-tostring-throws-symbol.js")]
    public Task this_value_tostring_throws_symbol() => ExecutionTestFromFile("this-value-tostring-throws-symbol");
    [Fact(DisplayName = "this-value-tostring-throws-toprimitive.js")]
    public Task this_value_tostring_throws_toprimitive() => ExecutionTestFromFile("this-value-tostring-throws-toprimitive");
}
