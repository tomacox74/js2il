using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.addition;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.addition") { }

    [Fact(DisplayName = "coerce-bigint-to-string")]
    public Task coerce_bigint_to_string()
        => ExecutionTest("coerce-bigint-to-string");

    [Fact(DisplayName = "coerce-symbol-to-prim-invocation")]
    public Task coerce_symbol_to_prim_invocation()
        => ExecutionTest("coerce-symbol-to-prim-invocation");
}
