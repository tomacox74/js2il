using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.class_;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_") { }

[Fact(DisplayName = "accessor-name-inst-computed-yield-expr", Skip = "Computed class accessor names with yield are not compiled yet.")]
    public Task accessor_name_inst_computed_yield_expr()
        => ExecutionTest("accessor-name-inst-computed-yield-expr");

[Fact(DisplayName = "accessor-name-static-computed-yield-expr", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task accessor_name_static_computed_yield_expr()
        => ExecutionTest("accessor-name-static-computed-yield-expr");
}
