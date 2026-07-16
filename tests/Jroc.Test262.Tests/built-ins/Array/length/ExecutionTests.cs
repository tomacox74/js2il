using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.length;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.length") { }

    [Fact(DisplayName = "define-own-prop-length-overflow-order")]
    public Task define_own_prop_length_overflow_order()
        => ExecutionTestFromFile("define-own-prop-length-overflow-order");

    [Fact(DisplayName = "define-own-prop-length-no-value-order")]
    public Task define_own_prop_length_no_value_order()
        => ExecutionTestFromFile("define-own-prop-length-no-value-order");
}
