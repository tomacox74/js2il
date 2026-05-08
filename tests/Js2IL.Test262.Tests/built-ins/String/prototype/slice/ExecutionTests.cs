using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.String.prototype.slice;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.slice") { }

    [Fact(DisplayName = "S15.5.4.13_A10", Skip = "String.prototype.slice boxed-string/object coercion handling is incomplete.")]
    public Task S15_5_4_13_A10()
        => ExecutionTestFromFile("S15.5.4.13_A10");

    [Fact(DisplayName = "S15.5.4.13_A11", Skip = "String.prototype.slice boxed-string/object coercion handling is incomplete.")]
    public Task S15_5_4_13_A11()
        => ExecutionTestFromFile("S15.5.4.13_A11");

    [Fact(DisplayName = "S15.5.4.13_A1_T1", Skip = "String.prototype.slice boxed-string/object coercion handling is incomplete.")]
    public Task S15_5_4_13_A1_T1()
        => ExecutionTestFromFile("S15.5.4.13_A1_T1");

    [Fact(DisplayName = "S15.5.4.13_A1_T10")]
    public Task S15_5_4_13_A1_T10()
        => ExecutionTestFromFile("S15.5.4.13_A1_T10");

    [Fact(DisplayName = "S15.5.4.13_A1_T11", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task S15_5_4_13_A1_T11()
        => ExecutionTestFromFile("S15.5.4.13_A1_T11");

    [Fact(DisplayName = "S15.5.4.13_A1_T12", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task S15_5_4_13_A1_T12()
        => ExecutionTestFromFile("S15.5.4.13_A1_T12");

    [Fact(DisplayName = "S15.5.4.13_A1_T13", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task S15_5_4_13_A1_T13()
        => ExecutionTestFromFile("S15.5.4.13_A1_T13");

    [Fact(DisplayName = "S15.5.4.13_A1_T14")]
    public Task S15_5_4_13_A1_T14()
        => ExecutionTestFromFile("S15.5.4.13_A1_T14");

    [Fact(DisplayName = "S15.5.4.13_A1_T15", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task S15_5_4_13_A1_T15()
        => ExecutionTestFromFile("S15.5.4.13_A1_T15");

    [Fact(DisplayName = "S15.5.4.13_A1_T2", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task S15_5_4_13_A1_T2()
        => ExecutionTestFromFile("S15.5.4.13_A1_T2");

    [Fact(DisplayName = "S15.5.4.13_A1_T4")]
    public Task S15_5_4_13_A1_T4()
        => ExecutionTestFromFile("S15.5.4.13_A1_T4");

    [Fact(DisplayName = "S15.5.4.13_A1_T5", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task S15_5_4_13_A1_T5()
        => ExecutionTestFromFile("S15.5.4.13_A1_T5");

    [Fact(DisplayName = "S15.5.4.13_A1_T6", Skip = "Known issue: runtime behavior diverges from test262 expectation")]
    public Task S15_5_4_13_A1_T6()
        => ExecutionTestFromFile("S15.5.4.13_A1_T6");

    [Fact(DisplayName = "S15.5.4.13_A1_T7", Skip = "Known issue: runtime behavior diverges from test262 expectation")]
    public Task S15_5_4_13_A1_T7()
        => ExecutionTestFromFile("S15.5.4.13_A1_T7");

    [Fact(DisplayName = "S15.5.4.13_A1_T8", Skip = "Known issue: runtime failure in this test262 scenario")]
    public Task S15_5_4_13_A1_T8()
        => ExecutionTestFromFile("S15.5.4.13_A1_T8");

    [Fact(DisplayName = "S15.5.4.13_A1_T9", Skip = "Known issue: runtime behavior diverges from test262 expectation")]
    public Task S15_5_4_13_A1_T9()
        => ExecutionTestFromFile("S15.5.4.13_A1_T9");
}
