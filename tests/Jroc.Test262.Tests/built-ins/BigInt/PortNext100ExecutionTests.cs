using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.BigInt;

public class PortNext100ExecutionTests : DiskExecutionTestsBase
{
    public PortNext100ExecutionTests() : base("built_ins.BigInt") { }

    [Fact(DisplayName = "asIntN/bigint-tobigint")]
    public Task asIntN_bigint_tobigint()
        => ExecutionTestFromFile("asIntN/bigint-tobigint");

    [Fact(DisplayName = "asIntN/bigint-tobigint-wrapped-values")]
    public Task asIntN_bigint_tobigint_wrapped_values()
        => ExecutionTestFromFile("asIntN/bigint-tobigint-wrapped-values");

    [Fact(DisplayName = "asIntN/bits-toindex")]
    public Task asIntN_bits_toindex()
        => ExecutionTestFromFile("asIntN/bits-toindex");

    [Fact(DisplayName = "asIntN/bits-toindex-wrapped-values")]
    public Task asIntN_bits_toindex_wrapped_values()
        => ExecutionTestFromFile("asIntN/bits-toindex-wrapped-values");

    [Fact(DisplayName = "asIntN/order-of-steps")]
    public Task asIntN_order_of_steps()
        => ExecutionTestFromFile("asIntN/order-of-steps");

    [Fact(DisplayName = "asUintN/bigint-tobigint")]
    public Task asUintN_bigint_tobigint()
        => ExecutionTestFromFile("asUintN/bigint-tobigint");

    [Fact(DisplayName = "asUintN/bigint-tobigint-wrapped-values")]
    public Task asUintN_bigint_tobigint_wrapped_values()
        => ExecutionTestFromFile("asUintN/bigint-tobigint-wrapped-values");

    [Fact(DisplayName = "asUintN/bits-toindex")]
    public Task asUintN_bits_toindex()
        => ExecutionTestFromFile("asUintN/bits-toindex");

    [Fact(DisplayName = "asUintN/bits-toindex-wrapped-values")]
    public Task asUintN_bits_toindex_wrapped_values()
        => ExecutionTestFromFile("asUintN/bits-toindex-wrapped-values");

    [Fact(DisplayName = "asUintN/order-of-steps")]
    public Task asUintN_order_of_steps()
        => ExecutionTestFromFile("asUintN/order-of-steps");

}
