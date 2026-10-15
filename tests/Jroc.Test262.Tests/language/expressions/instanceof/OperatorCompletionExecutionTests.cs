using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.instanceof;

public class OperatorCompletionExecutionTests : DiskExecutionTestsBase
{
    public OperatorCompletionExecutionTests() : base("language.expressions.instanceof") { }

    [Fact(DisplayName = "S11.8.6_A1.js", Skip = "eval is not supported.")]
    public Task test_S11_8_6_A1() => ExecutionTest("S11.8.6_A1");

    [Fact(DisplayName = "symbol-hasinstance-get-err.js")]
    public Task test_symbol_hasinstance_get_err() => ExecutionTest("symbol-hasinstance-get-err");

    [Fact(DisplayName = "symbol-hasinstance-invocation.js")]
    public Task test_symbol_hasinstance_invocation() => ExecutionTest("symbol-hasinstance-invocation");

    [Fact(DisplayName = "symbol-hasinstance-to-boolean.js")]
    public Task test_symbol_hasinstance_to_boolean() => ExecutionTest("symbol-hasinstance-to-boolean");
}
