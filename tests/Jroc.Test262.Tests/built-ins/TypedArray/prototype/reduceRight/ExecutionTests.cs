using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.reduceRight;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("TypedArray.prototype.reduceRight") { }

    [Fact(DisplayName = "callbackfn-arguments-custom-accumulator")]
    public Task callbackfn_arguments_custom_accumulator() => ExecutionTestFromFile("callbackfn-arguments-custom-accumulator");

    [Fact(DisplayName = "callbackfn-arguments-default-accumulator")]
    public Task callbackfn_arguments_default_accumulator() => ExecutionTestFromFile("callbackfn-arguments-default-accumulator");

    [Fact(DisplayName = "callbackfn-no-iteration-over-non-integer-properties")]
    public Task callbackfn_no_iteration_over_non_integer_properties() => ExecutionTestFromFile("callbackfn-no-iteration-over-non-integer-properties");

    [Fact(DisplayName = "callbackfn-not-called-on-empty")]
    public Task callbackfn_not_called_on_empty() => ExecutionTestFromFile("callbackfn-not-called-on-empty");

    [Fact(DisplayName = "callbackfn-return-does-not-change-instance")]
    public Task callbackfn_return_does_not_change_instance() => ExecutionTestFromFile("callbackfn-return-does-not-change-instance");

    [Fact(DisplayName = "callbackfn-returns-abrupt")]
    public Task callbackfn_returns_abrupt() => ExecutionTestFromFile("callbackfn-returns-abrupt");

    [Fact(DisplayName = "callbackfn-set-value-during-iteration")]
    public Task callbackfn_set_value_during_iteration() => ExecutionTestFromFile("callbackfn-set-value-during-iteration");

    [Fact(DisplayName = "callbackfn-this")]
    public Task callbackfn_this() => ExecutionTestFromFile("callbackfn-this");

    [Fact(DisplayName = "empty-instance-return-initialvalue")]
    public Task empty_instance_return_initialvalue() => ExecutionTestFromFile("empty-instance-return-initialvalue");

    [Fact(DisplayName = "get-length-uses-internal-arraylength")]
    public Task get_length_uses_internal_arraylength() => ExecutionTestFromFile("get-length-uses-internal-arraylength");

    [Fact(DisplayName = "invoked-as-func")]
    public Task invoked_as_func() => ExecutionTestFromFile("invoked-as-func");

    [Fact(DisplayName = "invoked-as-method")]
    public Task invoked_as_method() => ExecutionTestFromFile("invoked-as-method");

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "result-is-last-callbackfn-return")]
    public Task result_is_last_callbackfn_return() => ExecutionTestFromFile("result-is-last-callbackfn-return");

    [Fact(DisplayName = "result-of-any-type")]
    public Task result_of_any_type() => ExecutionTestFromFile("result-of-any-type");

    [Fact(DisplayName = "return-first-value-without-callbackfn")]
    public Task return_first_value_without_callbackfn() => ExecutionTestFromFile("return-first-value-without-callbackfn");

    [Fact(DisplayName = "values-are-not-cached")]
    public Task values_are_not_cached() => ExecutionTestFromFile("values-are-not-cached");
}
