namespace Jroc.Test262.Tests.language.expressions.addition;

public class AdditionCompletionExecutionTests : DiskExecutionTestsBase
{
    public AdditionCompletionExecutionTests() : base("language.expressions.addition") { }

    [Fact(DisplayName = "S11.6.1_A1.js", Skip = "eval is not supported.")]
    public Task S11_6_1_A1() => ExecutionTest("S11.6.1_A1");

    [Fact(DisplayName = "bigint-toprimitive.js")]
    public Task bigint_toprimitive() => ExecutionTest("bigint-toprimitive");

    [Fact(DisplayName = "coerce-symbol-to-prim-return-obj.js")]
    public Task coerce_symbol_to_prim_return_obj() => ExecutionTest("coerce-symbol-to-prim-return-obj");
}
