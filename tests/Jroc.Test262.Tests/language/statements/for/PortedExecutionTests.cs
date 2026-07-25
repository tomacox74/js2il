using Jroc.Tests;

namespace Jroc.Test262.Tests.language.statements.@for;

public sealed class PortedExecutionTests : ExecutionTestsBase
{
    public PortedExecutionTests() : base("language.statements.for") { }

    [Fact(DisplayName = "tco-const-body")]
    public Task tco_const_body()
        => ExecutionTest("tco-const-body");

    [Fact(DisplayName = "tco-let-body")]
    public Task tco_let_body()
        => ExecutionTest("tco-let-body");

    [Fact(DisplayName = "tco-lhs-body")]
    public Task tco_lhs_body()
        => ExecutionTest("tco-lhs-body");

    [Fact(DisplayName = "tco-var-body")]
    public Task tco_var_body()
        => ExecutionTest("tco-var-body");

}
