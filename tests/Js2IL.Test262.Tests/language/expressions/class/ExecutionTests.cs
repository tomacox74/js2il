using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.class_;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.class_") { }

[Fact(DisplayName = "accessor-name-inst-computed-in", Skip = "Computed class accessor names using in are not compiled yet.")]
    public Task accessor_name_inst_computed_in()
        => ExecutionTest("accessor-name-inst-computed-in");

[Fact(DisplayName = "accessor-name-inst-computed-yield-expr", Skip = "Computed class accessor names with yield are not compiled yet.")]
    public Task accessor_name_inst_computed_yield_expr()
        => ExecutionTest("accessor-name-inst-computed-yield-expr");

[Fact(DisplayName = "accessor-name-static-computed-in", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task accessor_name_static_computed_in()
        => ExecutionTest("accessor-name-static-computed-in");

[Fact(DisplayName = "accessor-name-static-computed-yield-expr", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task accessor_name_static_computed_yield_expr()
        => ExecutionTest("accessor-name-static-computed-yield-expr");

[Fact(DisplayName = "accessor-name-inst-computed-in", Skip = "Computed class accessor names using in are not compiled yet.")]
    public Task accessor_name_inst_computed_in()
        => ExecutionTest("accessor-name-inst-computed-in");

[Fact(DisplayName = "accessor-name-inst-computed-yield-expr", Skip = "Computed class accessor names with yield are not compiled yet.")]
    public Task accessor_name_inst_computed_yield_expr()
        => ExecutionTest("accessor-name-inst-computed-yield-expr");

[Fact(DisplayName = "accessor-name-static-computed-in", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task accessor_name_static_computed_in()
        => ExecutionTest("accessor-name-static-computed-in");

[Fact(DisplayName = "accessor-name-static-computed-yield-expr", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task accessor_name_static_computed_yield_expr()
        => ExecutionTest("accessor-name-static-computed-yield-expr");

[Fact(DisplayName = "class-name-ident-await-escaped", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task class_name_ident_await_escaped()
        => ExecutionTest("class-name-ident-await-escaped");

[Fact(DisplayName = "class-name-ident-await", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task class_name_ident_await()
        => ExecutionTest("class-name-ident-await");
}
