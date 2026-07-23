using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.lastIndexOf;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.lastIndexOf") { }

    [Fact(DisplayName = "name")] public Task name() => ExecutionTestFromFile("name");
    [Fact(DisplayName = "not-a-constructor")] public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
    [Fact(DisplayName = "not-a-substring")] public Task not_a_substring() => ExecutionTestFromFile("not-a-substring");
    [Fact(DisplayName = "this-value-not-obj-coercible")] public Task this_value_not_obj_coercible() => ExecutionTestFromFile("this-value-not-obj-coercible");
    [Fact(DisplayName = "S15.5.4.8_A10")] public Task S15_5_4_8_A10() => ExecutionTestFromFile("S15.5.4.8_A10");
    [Fact(DisplayName = "S15.5.4.8_A11")] public Task S15_5_4_8_A11() => ExecutionTestFromFile("S15.5.4.8_A11");
    [Fact(DisplayName = "S15.5.4.8_A1_T1")] public Task S15_5_4_8_A1_T1() => ExecutionTestFromFile("S15.5.4.8_A1_T1");
    [Fact(DisplayName = "S15.5.4.8_A1_T2")] public Task S15_5_4_8_A1_T2() => ExecutionTestFromFile("S15.5.4.8_A1_T2");
    [Fact(DisplayName = "S15.5.4.8_A1_T4")] public Task S15_5_4_8_A1_T4() => ExecutionTestFromFile("S15.5.4.8_A1_T4");
    [Fact(DisplayName = "S15.5.4.8_A1_T5")] public Task S15_5_4_8_A1_T5() => ExecutionTestFromFile("S15.5.4.8_A1_T5");
    [Fact(DisplayName = "S15.5.4.8_A1_T6")] public Task S15_5_4_8_A1_T6() => ExecutionTestFromFile("S15.5.4.8_A1_T6");
    [Fact(DisplayName = "S15.5.4.8_A1_T7")] public Task S15_5_4_8_A1_T7() => ExecutionTestFromFile("S15.5.4.8_A1_T7");
    [Fact(DisplayName = "S15.5.4.8_A1_T8")] public Task S15_5_4_8_A1_T8() => ExecutionTestFromFile("S15.5.4.8_A1_T8");
    [Fact(DisplayName = "S15.5.4.8_A1_T9")] public Task S15_5_4_8_A1_T9() => ExecutionTestFromFile("S15.5.4.8_A1_T9");
    [Fact(DisplayName = "S15.5.4.8_A1_T10")] public Task S15_5_4_8_A1_T10() => ExecutionTestFromFile("S15.5.4.8_A1_T10");
    [Fact(DisplayName = "S15.5.4.8_A1_T12")] public Task S15_5_4_8_A1_T12() => ExecutionTestFromFile("S15.5.4.8_A1_T12");
    [Fact(DisplayName = "S15.5.4.8_A4_T1")] public Task S15_5_4_8_A4_T1() => ExecutionTestFromFile("S15.5.4.8_A4_T1");
    [Fact(DisplayName = "S15.5.4.8_A4_T2")] public Task S15_5_4_8_A4_T2() => ExecutionTestFromFile("S15.5.4.8_A4_T2");
    [Fact(DisplayName = "S15.5.4.8_A4_T3")] public Task S15_5_4_8_A4_T3() => ExecutionTestFromFile("S15.5.4.8_A4_T3");
    [Fact(DisplayName = "S15.5.4.8_A4_T4")] public Task S15_5_4_8_A4_T4() => ExecutionTestFromFile("S15.5.4.8_A4_T4");
}
