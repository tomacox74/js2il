using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.block_scope.leave;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.block_scope.leave") { }

    [Fact(DisplayName = "finally-block-let-declaration-only-shadows-outer-parameter-value-1")]
    public Task finally_block_let_declaration_only_shadows_outer_parameter_value_1()
        => ExecutionTest("finally-block-let-declaration-only-shadows-outer-parameter-value-1");

    [Fact(DisplayName = "finally-block-let-declaration-only-shadows-outer-parameter-value-2")]
    public Task finally_block_let_declaration_only_shadows_outer_parameter_value_2()
        => ExecutionTest("finally-block-let-declaration-only-shadows-outer-parameter-value-2");

    [Fact(DisplayName = "for-loop-block-let-declaration-only-shadows-outer-parameter-value-1")]
    public Task for_loop_block_let_declaration_only_shadows_outer_parameter_value_1()
        => ExecutionTest("for-loop-block-let-declaration-only-shadows-outer-parameter-value-1");

    [Fact(DisplayName = "for-loop-block-let-declaration-only-shadows-outer-parameter-value-2")]
    public Task for_loop_block_let_declaration_only_shadows_outer_parameter_value_2()
        => ExecutionTest("for-loop-block-let-declaration-only-shadows-outer-parameter-value-2");

    [Fact(DisplayName = "nested-block-let-declaration-only-shadows-outer-parameter-value-1")]
    public Task nested_block_let_declaration_only_shadows_outer_parameter_value_1()
        => ExecutionTest("nested-block-let-declaration-only-shadows-outer-parameter-value-1");

    [Fact(DisplayName = "nested-block-let-declaration-only-shadows-outer-parameter-value-2")]
    public Task nested_block_let_declaration_only_shadows_outer_parameter_value_2()
        => ExecutionTest("nested-block-let-declaration-only-shadows-outer-parameter-value-2");

    [Fact(DisplayName = "try-block-let-declaration-only-shadows-outer-parameter-value-1")]
    public Task try_block_let_declaration_only_shadows_outer_parameter_value_1()
        => ExecutionTest("try-block-let-declaration-only-shadows-outer-parameter-value-1");

    [Fact(DisplayName = "x-after-break-to-label")]
    public Task x_after_break_to_label()
        => ExecutionTest("x-after-break-to-label");

    [Fact(DisplayName = "x-before-continue")]
    public Task x_before_continue()
        => ExecutionTest("x-before-continue");

    [Fact(DisplayName = "outermost-binding-updated-in-catch-block-nested-block-let-declaration-unseen-outside-of-block", Skip = "eval is not supported by JS2IL.")]
    public Task outermost_binding_updated_in_catch_block_nested_block_let_declaration_unseen_outside_of_block()
        => ExecutionTest("outermost-binding-updated-in-catch-block-nested-block-let-declaration-unseen-outside-of-block");

    [Fact(DisplayName = "try-block-let-declaration-only-shadows-outer-parameter-value-2")]
    public Task try_block_let_declaration_only_shadows_outer_parameter_value_2()
        => ExecutionTest("try-block-let-declaration-only-shadows-outer-parameter-value-2");

    [Fact(DisplayName = "verify-context-in-finally-block")]
    public Task verify_context_in_finally_block()
        => ExecutionTest("verify-context-in-finally-block");

    [Fact(DisplayName = "verify-context-in-for-loop-block")]
    public Task verify_context_in_for_loop_block()
        => ExecutionTest("verify-context-in-for-loop-block");

    [Fact(DisplayName = "verify-context-in-labelled-block")]
    public Task verify_context_in_labelled_block()
        => ExecutionTest("verify-context-in-labelled-block");
    [Fact(DisplayName = "verify-context-in-try-block")]
    public Task verify_context_in_try_block()
        => ExecutionTest("verify-context-in-try-block");

}
