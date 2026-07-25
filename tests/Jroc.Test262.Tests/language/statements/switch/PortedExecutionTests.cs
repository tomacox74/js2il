using Jroc.Tests;

namespace Jroc.Test262.Tests.language.statements.@switch;

public sealed class PortedExecutionTests : ExecutionTestsBase
{
    public PortedExecutionTests() : base("language.statements.switch") { }

    [Fact(DisplayName = "tco-case-body-dflt")]
    public Task tco_case_body_dflt()
        => ExecutionTest("tco-case-body-dflt");

    [Fact(DisplayName = "tco-case-body")]
    public Task tco_case_body()
        => ExecutionTest("tco-case-body");

    [Fact(DisplayName = "tco-dftl-body")]
    public Task tco_dftl_body()
        => ExecutionTest("tco-dftl-body");

}
