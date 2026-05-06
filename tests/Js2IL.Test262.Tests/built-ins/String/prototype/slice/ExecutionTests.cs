using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.String.prototype.slice;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.slice") { }

[Fact(DisplayName = "S15.5.4.13_A10", Skip = "String.prototype.slice boxed-string/object coercion handling is incomplete.")]
    public Task S15_5_4_13_A10()
        => ExecutionTest("S15.5.4.13_A10");

[Fact(DisplayName = "S15.5.4.13_A11", Skip = "String.prototype.slice boxed-string/object coercion handling is incomplete.")]
    public Task S15_5_4_13_A11()
        => ExecutionTest("S15.5.4.13_A11");

[Fact(DisplayName = "S15.5.4.13_A1_T1", Skip = "String.prototype.slice boxed-string/object coercion handling is incomplete.")]
    public Task S15_5_4_13_A1_T1()
        => ExecutionTest("S15.5.4.13_A1_T1");

[Fact(DisplayName = "S15.5.4.13_A1_T10", Skip = "String.prototype.slice boxed-string/object coercion handling is incomplete.")]
    public Task S15_5_4_13_A1_T10()
        => ExecutionTest("S15.5.4.13_A1_T10");

[Fact(DisplayName = "S15.5.4.13_A1_T11", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task S15_5_4_13_A1_T11()
        => ExecutionTest("S15.5.4.13_A1_T11");

[Fact(DisplayName = "S15.5.4.13_A1_T12", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task S15_5_4_13_A1_T12()
        => ExecutionTest("S15.5.4.13_A1_T12");

[Fact(DisplayName = "S15.5.4.13_A1_T13", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task S15_5_4_13_A1_T13()
        => ExecutionTest("S15.5.4.13_A1_T13");

[Fact(DisplayName = "S15.5.4.13_A1_T14")]
    public Task S15_5_4_13_A1_T14()
        => ExecutionTest("S15.5.4.13_A1_T14");

[Fact(DisplayName = "S15.5.4.13_A10", Skip = "String.prototype.slice boxed-string/object coercion handling is incomplete.")]
    public Task S15_5_4_13_A10()
        => ExecutionTest("S15.5.4.13_A10");

[Fact(DisplayName = "S15.5.4.13_A11", Skip = "String.prototype.slice boxed-string/object coercion handling is incomplete.")]
    public Task S15_5_4_13_A11()
        => ExecutionTest("S15.5.4.13_A11");

[Fact(DisplayName = "S15.5.4.13_A1_T1", Skip = "String.prototype.slice boxed-string/object coercion handling is incomplete.")]
    public Task S15_5_4_13_A1_T1()
        => ExecutionTest("S15.5.4.13_A1_T1");

[Fact(DisplayName = "S15.5.4.13_A1_T10", Skip = "String.prototype.slice boxed-string/object coercion handling is incomplete.")]
    public Task S15_5_4_13_A1_T10()
        => ExecutionTest("S15.5.4.13_A1_T10");

[Fact(DisplayName = "S15.5.4.13_A1_T11", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task S15_5_4_13_A1_T11()
        => ExecutionTest("S15.5.4.13_A1_T11");

[Fact(DisplayName = "S15.5.4.13_A1_T12", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task S15_5_4_13_A1_T12()
        => ExecutionTest("S15.5.4.13_A1_T12");

[Fact(DisplayName = "S15.5.4.13_A1_T13", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task S15_5_4_13_A1_T13()
        => ExecutionTest("S15.5.4.13_A1_T13");

[Fact(DisplayName = "S15.5.4.13_A1_T14")]
    public Task S15_5_4_13_A1_T14()
        => ExecutionTest("S15.5.4.13_A1_T14");

[Fact(DisplayName = "S15.5.4.13_A10", Skip = "String.prototype.slice boxed-string/object coercion handling is incomplete.")]
    public Task S15_5_4_13_A10()
        => ExecutionTest("S15.5.4.13_A10");

[Fact(DisplayName = "S15.5.4.13_A11", Skip = "String.prototype.slice boxed-string/object coercion handling is incomplete.")]
    public Task S15_5_4_13_A11()
        => ExecutionTest("S15.5.4.13_A11");

[Fact(DisplayName = "S15.5.4.13_A1_T1", Skip = "String.prototype.slice boxed-string/object coercion handling is incomplete.")]
    public Task S15_5_4_13_A1_T1()
        => ExecutionTest("S15.5.4.13_A1_T1");

[Fact(DisplayName = "S15.5.4.13_A1_T10", Skip = "String.prototype.slice boxed-string/object coercion handling is incomplete.")]
    public Task S15_5_4_13_A1_T10()
        => ExecutionTest("S15.5.4.13_A1_T10");

[Fact(DisplayName = "S15.5.4.13_A1_T11", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task S15_5_4_13_A1_T11()
        => ExecutionTest("S15.5.4.13_A1_T11");

[Fact(DisplayName = "S15.5.4.13_A1_T12", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task S15_5_4_13_A1_T12()
        => ExecutionTest("S15.5.4.13_A1_T12");

[Fact(DisplayName = "S15.5.4.13_A1_T13", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task S15_5_4_13_A1_T13()
        => ExecutionTest("S15.5.4.13_A1_T13");

[Fact(DisplayName = "S15.5.4.13_A1_T14")]
    public Task S15_5_4_13_A1_T14()
        => ExecutionTest("S15.5.4.13_A1_T14");
}
