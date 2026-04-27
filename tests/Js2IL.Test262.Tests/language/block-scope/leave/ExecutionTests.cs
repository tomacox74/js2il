using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.block_scope.leave;

public class ExecutionTests : ExecutionTestsBase
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
}
