using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.statements.class_.definition;

public class PortDefinitionExecutionTests : DiskExecutionTestsBase
{
    public PortDefinitionExecutionTests() : base("language.statements.class_.definition") { }

    [Fact(DisplayName = "constructor-property")]
    public Task constructor_property()
        => ExecutionTest("constructor-property");

    [Fact(DisplayName = "constructor-strict-by-default")]
    public Task constructor_strict_by_default()
        => ExecutionTest("constructor-strict-by-default");

    [Fact(DisplayName = "prototype-property")]
    public Task prototype_property()
        => ExecutionTest("prototype-property");

    [Fact(DisplayName = "getters-prop-desc")]
    public Task getters_prop_desc()
        => ExecutionTest("getters-prop-desc");
}
