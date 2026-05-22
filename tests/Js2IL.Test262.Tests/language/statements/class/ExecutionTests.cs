using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.class_;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_") { }

    [Fact(DisplayName = "accessor-name-inst-computed-yield-expr")]
    public Task accessor_name_inst_computed_yield_expr()
        => ExecutionTest("accessor-name-inst-computed-yield-expr", preferOutOfProc: true);

    [Fact(DisplayName = "accessor-name-static-computed-yield-expr")]
    public Task accessor_name_static_computed_yield_expr()
        => ExecutionTest("accessor-name-static-computed-yield-expr", preferOutOfProc: true);
}
