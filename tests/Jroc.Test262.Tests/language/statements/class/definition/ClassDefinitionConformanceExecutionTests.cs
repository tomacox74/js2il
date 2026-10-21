namespace Jroc.Test262.Tests.language.statements.test_class.definition;

public sealed class ClassDefinitionConformanceExecutionTests : DiskExecutionTestsBase
{
    public ClassDefinitionConformanceExecutionTests() : base("language.statements.test_class.definition") { }

    [Fact(DisplayName = "constructor.js")]
    public Task constructor()
        => ExecutionTest("constructor");

    [Fact(DisplayName = "fn-length-static-precedence-order.js")]
    public Task fn_length_static_precedence_order()
        => ExecutionTest("fn-length-static-precedence-order");

    [Fact(DisplayName = "fn-length-static-precedence.js")]
    public Task fn_length_static_precedence()
        => ExecutionTest("fn-length-static-precedence");

    [Fact(DisplayName = "fn-name-static-precedence-order.js")]
    public Task fn_name_static_precedence_order()
        => ExecutionTest("fn-name-static-precedence-order");

    [Fact(DisplayName = "getters-non-configurable-err.js")]
    public Task getters_non_configurable_err()
        => ExecutionTest("getters-non-configurable-err");

    [Fact(DisplayName = "implicit-constructor.js")]
    public Task implicit_constructor()
        => ExecutionTest("implicit-constructor");

    [Fact(DisplayName = "numeric-property-names.js")]
    public Task numeric_property_names()
        => ExecutionTest("numeric-property-names");

    [Fact(DisplayName = "prototype-wiring.js")]
    public Task prototype_wiring()
        => ExecutionTest("prototype-wiring");

    [Fact(DisplayName = "setters-non-configurable-err.js")]
    public Task setters_non_configurable_err()
        => ExecutionTest("setters-non-configurable-err");

    [Fact(DisplayName = "this-access-restriction-2.js")]
    public Task this_access_restriction_2()
        => ExecutionTest("this-access-restriction-2");
}
