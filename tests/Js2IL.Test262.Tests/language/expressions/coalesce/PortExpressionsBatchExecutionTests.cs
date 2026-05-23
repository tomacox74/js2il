using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.coalesce;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.coalesce") { }

    [Fact(DisplayName = "abrupt-is-a-short-circuit")]
    public Task abrupt_is_a_short_circuit()
        => ExecutionTest("abrupt-is-a-short-circuit");

    [Fact(DisplayName = "chainable-with-bitwise-or")]
    public Task chainable_with_bitwise_or()
        => ExecutionTest("chainable-with-bitwise-or");

    [Fact(DisplayName = "chainable-with-bitwise-xor")]
    public Task chainable_with_bitwise_xor()
        => ExecutionTest("chainable-with-bitwise-xor");

    [Fact(DisplayName = "chainable")]
    public Task chainable()
        => ExecutionTest("chainable");

    [Fact(DisplayName = "follows-null")]
    public Task follows_null()
        => ExecutionTest("follows-null");

    [Fact(DisplayName = "follows-undefined")]
    public Task follows_undefined()
        => ExecutionTest("follows-undefined");

    [Fact(DisplayName = "short-circuit-number-0")]
    public Task short_circuit_number_0()
        => ExecutionTest("short-circuit-number-0");

    [Fact(DisplayName = "short-circuit-number-empty-string")]
    public Task short_circuit_number_empty_string()
        => ExecutionTest("short-circuit-number-empty-string");

    [Fact(DisplayName = "short-circuit-number-false")]
    public Task short_circuit_number_false()
        => ExecutionTest("short-circuit-number-false");

    [Fact(DisplayName = "short-circuit-number-object")]
    public Task short_circuit_number_object()
        => ExecutionTest("short-circuit-number-object");

    [Fact(DisplayName = "short-circuit-number-string")]
    public Task short_circuit_number_string()
        => ExecutionTest("short-circuit-number-string");

    [Fact(DisplayName = "short-circuit-number-true")]
    public Task short_circuit_number_true()
        => ExecutionTest("short-circuit-number-true");
}
