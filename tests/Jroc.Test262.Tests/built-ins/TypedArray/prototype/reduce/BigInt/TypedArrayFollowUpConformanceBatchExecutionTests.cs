using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.reduce.BigInt;

public class TypedArrayFollowUpConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayFollowUpConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.reduce.BigInt") { }

    [Fact(DisplayName = "callbackfn-arguments-custom-accumulator.js")]
    public Task callbackfn_arguments_custom_accumulator() => ExecutionTestFromFile("callbackfn-arguments-custom-accumulator");

    [Fact(DisplayName = "callbackfn-arguments-default-accumulator.js")]
    public Task callbackfn_arguments_default_accumulator() => ExecutionTestFromFile("callbackfn-arguments-default-accumulator");

    [Fact(DisplayName = "callbackfn-is-not-callable-throws.js")]
    public Task callbackfn_is_not_callable_throws() => ExecutionTestFromFile("callbackfn-is-not-callable-throws");

    [Fact(DisplayName = "callbackfn-no-iteration-over-non-integer-properties.js")]
    public Task callbackfn_no_iteration_over_non_integer_properties() => ExecutionTestFromFile("callbackfn-no-iteration-over-non-integer-properties");

    [Fact(DisplayName = "callbackfn-not-called-on-empty.js")]
    public Task callbackfn_not_called_on_empty() => ExecutionTestFromFile("callbackfn-not-called-on-empty");

    [Fact(DisplayName = "callbackfn-return-does-not-change-instance.js")]
    public Task callbackfn_return_does_not_change_instance() => ExecutionTestFromFile("callbackfn-return-does-not-change-instance");

    [Fact(DisplayName = "callbackfn-returns-abrupt.js")]
    public Task callbackfn_returns_abrupt() => ExecutionTestFromFile("callbackfn-returns-abrupt");

    [Fact(DisplayName = "callbackfn-set-value-during-iteration.js")]
    public Task callbackfn_set_value_during_iteration() => ExecutionTestFromFile("callbackfn-set-value-during-iteration");

    [Fact(DisplayName = "callbackfn-this.js")]
    public Task callbackfn_this() => ExecutionTestFromFile("callbackfn-this");

    [Fact(DisplayName = "empty-instance-return-initialvalue.js")]
    public Task empty_instance_return_initialvalue() => ExecutionTestFromFile("empty-instance-return-initialvalue");

    [Fact(DisplayName = "empty-instance-with-no-initialvalue-throws.js")]
    public Task empty_instance_with_no_initialvalue_throws() => ExecutionTestFromFile("empty-instance-with-no-initialvalue-throws");

    [Fact(DisplayName = "get-length-uses-internal-arraylength.js")]
    public Task get_length_uses_internal_arraylength() => ExecutionTestFromFile("get-length-uses-internal-arraylength");

    [Fact(DisplayName = "result-is-last-callbackfn-return.js")]
    public Task result_is_last_callbackfn_return() => ExecutionTestFromFile("result-is-last-callbackfn-return");

    [Fact(DisplayName = "result-of-any-type.js")]
    public Task result_of_any_type() => ExecutionTestFromFile("result-of-any-type");

}
