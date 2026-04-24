using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.arrow_function;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.arrow_function") { }

    [Fact(DisplayName = "ArrowFunction_restricted-properties")]
    public Task ArrowFunction_restricted_properties()
        => ExecutionTest("ArrowFunction_restricted-properties");
}
