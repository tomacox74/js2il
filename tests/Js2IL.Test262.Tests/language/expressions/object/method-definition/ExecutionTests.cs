using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.object_.method_definition;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.object_.method_definition") { }

    [Fact(DisplayName = "computed-property-name-yield-expression", Skip = "Computed object property names with yield are not compiled correctly yet.")]
    public Task computed_property_name_yield_expression()
        => ExecutionTest("computed-property-name-yield-expression");
}
