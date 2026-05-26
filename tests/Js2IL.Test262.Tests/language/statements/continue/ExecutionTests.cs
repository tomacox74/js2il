using Js2IL.Test262.Tests.language.statements;

namespace Js2IL.Test262.Tests.language.statements.continue_;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\statements\continue", "language.statements.continue_") { }

    [Fact(DisplayName = "12.7-1")]
    public Task _12_7_1()
        => ExecutionTest("12.7-1");

    [Fact(DisplayName = "labeled-continue")]
    public Task labeled_continue()
        => ExecutionTest("labeled-continue");

    [Fact(DisplayName = "no-label-continue")]
    public Task no_label_continue()
        => ExecutionTest("no-label-continue");

    [Fact(DisplayName = "simple-and-labeled")]
    public Task simple_and_labeled()
        => ExecutionTest("simple-and-labeled");
    [Fact(DisplayName = "nested-let-bound-for-loops-inner-continue")]
    public Task nested_let_bound_for_loops_inner_continue()
        => ExecutionTest("nested-let-bound-for-loops-inner-continue");

    [Fact(DisplayName = "nested-let-bound-for-loops-labeled-continue")]
    public Task nested_let_bound_for_loops_labeled_continue()
        => ExecutionTest("nested-let-bound-for-loops-labeled-continue");

    [Fact(DisplayName = "nested-let-bound-for-loops-outer-continue")]
    public Task nested_let_bound_for_loops_outer_continue()
        => ExecutionTest("nested-let-bound-for-loops-outer-continue");

    [Fact(DisplayName = "shadowing-loop-variable-in-same-scope-as-continue")]
    public Task shadowing_loop_variable_in_same_scope_as_continue()
        => ExecutionTest("shadowing-loop-variable-in-same-scope-as-continue");



    [Fact(DisplayName = "line-terminators")]
    public Task line_terminators()
        => ExecutionTest("line-terminators");

    [Fact(DisplayName = "S12.7_A9_T1")]
    public Task S12_7_A9_T1()
        => ExecutionTest("S12.7_A9_T1");

    [Fact(DisplayName = "S12.7_A9_T2")]
    public Task S12_7_A9_T2()
        => ExecutionTest("S12.7_A9_T2");

    [Fact(DisplayName = "S12.7_A1_T1")]
    public Task S12_7_A1_T1()
        => CompilationFailureTest("S12.7_A1_T1", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.7_A1_T2")]
    public Task S12_7_A1_T2()
        => CompilationFailureTest("S12.7_A1_T2", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.7_A5_T1")]
    public Task S12_7_A5_T1()
        => CompilationFailureTest("S12.7_A5_T1", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.7_A6")]
    public Task S12_7_A6()
        => CompilationFailureTest("S12.7_A6", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.7_A5_T2")]
    public Task S12_7_A5_T2()
        => CompilationFailureTest("S12.7_A5_T2", "Failed to parse JavaScript");

}
