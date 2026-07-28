using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.sort;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("TypedArray.prototype.sort") { }

    [Fact(DisplayName = "arraylength-internal")]
    public Task arraylength_internal() => ExecutionTestFromFile("arraylength-internal");

    [Fact(DisplayName = "comparefn-call-throws")]
    public Task comparefn_call_throws() => ExecutionTestFromFile("comparefn-call-throws");

    [Fact(DisplayName = "comparefn-calls")]
    public Task comparefn_calls() => ExecutionTestFromFile("comparefn-calls");

    [Fact(DisplayName = "comparefn-is-undefined")]
    public Task comparefn_is_undefined() => ExecutionTestFromFile("comparefn-is-undefined");

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

    [Fact(DisplayName = "return-same-instance")]
    public Task return_same_instance() => ExecutionTestFromFile("return-same-instance");

    [Fact(DisplayName = "sorted-values-nan")]
    public Task sorted_values_nan() => ExecutionTestFromFile("sorted-values-nan");

    [Fact(DisplayName = "sorted-values")]
    public Task sorted_values() => ExecutionTestFromFile("sorted-values");

    [Fact(DisplayName = "sortcompare-with-no-tostring")]
    public Task sortcompare_with_no_tostring() => ExecutionTestFromFile("sortcompare-with-no-tostring");

    [Fact(DisplayName = "stability")]
    public Task stability() => ExecutionTestFromFile("stability");

    [Fact(DisplayName = "this-is-not-object")]
    public Task this_is_not_object() => ExecutionTestFromFile("this-is-not-object");

    [Fact(DisplayName = "this-is-not-typedarray-instance")]
    public Task this_is_not_typedarray_instance() => ExecutionTestFromFile("this-is-not-typedarray-instance");
}
