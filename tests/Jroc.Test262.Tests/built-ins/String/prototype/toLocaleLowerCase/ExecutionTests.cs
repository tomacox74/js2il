using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.toLocaleLowerCase;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.toLocaleLowerCase") { }

    [Fact(DisplayName = "name")] public Task name() => ExecutionTestFromFile("name");
    [Fact(DisplayName = "not-a-constructor")] public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
    [Fact(DisplayName = "this-value-not-obj-coercible")] public Task this_value_not_obj_coercible() => ExecutionTestFromFile("this-value-not-obj-coercible");
    [Fact(DisplayName = "S15.5.4.17_A10")] public Task S15_5_4_17_A10() => ExecutionTestFromFile("S15.5.4.17_A10");
    [Fact(DisplayName = "S15.5.4.17_A11")] public Task S15_5_4_17_A11() => ExecutionTestFromFile("S15.5.4.17_A11");
    [Fact(DisplayName = "S15.5.4.17_A1_T1")] public Task S15_5_4_17_A1_T1() => ExecutionTestFromFile("S15.5.4.17_A1_T1");
    [Fact(DisplayName = "S15.5.4.17_A1_T2")] public Task S15_5_4_17_A1_T2() => ExecutionTestFromFile("S15.5.4.17_A1_T2");
    [Fact(DisplayName = "S15.5.4.17_A6")] public Task S15_5_4_17_A6() => ExecutionTestFromFile("S15.5.4.17_A6");
    [Fact(DisplayName = "S15.5.4.17_A1_T4")] public Task S15_5_4_17_A1_T4() => ExecutionTestFromFile("S15.5.4.17_A1_T4");
    [Fact(DisplayName = "S15.5.4.17_A1_T5")] public Task S15_5_4_17_A1_T5() => ExecutionTestFromFile("S15.5.4.17_A1_T5");
    [Fact(DisplayName = "S15.5.4.17_A1_T6")] public Task S15_5_4_17_A1_T6() => ExecutionTestFromFile("S15.5.4.17_A1_T6");
    [Fact(DisplayName = "S15.5.4.17_A1_T7")] public Task S15_5_4_17_A1_T7() => ExecutionTestFromFile("S15.5.4.17_A1_T7");
    [Fact(DisplayName = "S15.5.4.17_A1_T8")] public Task S15_5_4_17_A1_T8() => ExecutionTestFromFile("S15.5.4.17_A1_T8");
    [Fact(DisplayName = "S15.5.4.17_A1_T9")] public Task S15_5_4_17_A1_T9() => ExecutionTestFromFile("S15.5.4.17_A1_T9");
    [Fact(DisplayName = "S15.5.4.17_A1_T10")] public Task S15_5_4_17_A1_T10() => ExecutionTestFromFile("S15.5.4.17_A1_T10");
    [Fact(DisplayName = "S15.5.4.17_A1_T11")] public Task S15_5_4_17_A1_T11() => ExecutionTestFromFile("S15.5.4.17_A1_T11");
    [Fact(DisplayName = "S15.5.4.17_A1_T12")] public Task S15_5_4_17_A1_T12() => ExecutionTestFromFile("S15.5.4.17_A1_T12");
    [Fact(DisplayName = "S15.5.4.17_A1_T13")] public Task S15_5_4_17_A1_T13() => ExecutionTestFromFile("S15.5.4.17_A1_T13");
    [Fact(DisplayName = "S15.5.4.17_A1_T14")] public Task S15_5_4_17_A1_T14() => ExecutionTestFromFile("S15.5.4.17_A1_T14");
    [Fact(DisplayName = "S15.5.4.17_A2_T1")] public Task S15_5_4_17_A2_T1() => ExecutionTestFromFile("S15.5.4.17_A2_T1");
}
