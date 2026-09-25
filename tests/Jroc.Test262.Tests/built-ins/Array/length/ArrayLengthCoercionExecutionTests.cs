using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.length;

public class ArrayLengthCoercionExecutionTests : InMemoryExecutionTestsBase
{
    public ArrayLengthCoercionExecutionTests() : base("built_ins.Array.length") { }

    [Fact(DisplayName = "define-own-prop-length-coercion-order-set.js")]
    public Task define_own_prop_length_coercion_order_set()
        => ExecutionTestFromFile("define-own-prop-length-coercion-order-set");

    [Fact(DisplayName = "define-own-prop-length-coercion-order.js")]
    public Task define_own_prop_length_coercion_order()
        => ExecutionTestFromFile("define-own-prop-length-coercion-order");
}
