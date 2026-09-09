using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.includes;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("TypedArray.prototype.includes") { }

    [Fact(DisplayName = "fromIndex-equal-or-greater-length-returns-false")]
    public Task fromIndex_equal_or_greater_length_returns_false()
        => ExecutionTestFromFile("fromIndex-equal-or-greater-length-returns-false");

    [Fact(DisplayName = "search-not-found-returns-false")]
    public Task search_not_found_returns_false()
        => ExecutionTestFromFile("search-not-found-returns-false");

    [Fact(DisplayName = "coerced-searchelement-fromindex-resize")]
    public Task coerced_searchelement_fromindex_resize()
        => ExecutionTestFromFile("coerced-searchelement-fromindex-resize");

    [Fact(DisplayName = "detached-buffer-during-fromIndex-returns-false-for-zero")]
    public Task detached_buffer_during_fromIndex_returns_false_for_zero()
        => ExecutionTestFromFile("detached-buffer-during-fromIndex-returns-false-for-zero");

    [Fact(DisplayName = "detached-buffer-during-fromIndex-returns-true-for-undefined")]
    public Task detached_buffer_during_fromIndex_returns_true_for_undefined()
        => ExecutionTestFromFile("detached-buffer-during-fromIndex-returns-true-for-undefined");

    [Fact(DisplayName = "detached-buffer")]
    public Task detached_buffer()
        => ExecutionTestFromFile("detached-buffer");

    [Fact(DisplayName = "invoked-as-func")]
    public Task invoked_as_func()
        => ExecutionTestFromFile("invoked-as-func");

    [Fact(DisplayName = "invoked-as-method")]
    public Task invoked_as_method()
        => ExecutionTestFromFile("invoked-as-method");

    [Fact(DisplayName = "length-zero-returns-false")]
    public Task length_zero_returns_false()
        => ExecutionTestFromFile("length-zero-returns-false");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "resizable-buffer-special-float-values")]
    public Task resizable_buffer_special_float_values()
        => ExecutionTestFromFile("resizable-buffer-special-float-values");

    [Fact(DisplayName = "resizable-buffer")]
    public Task resizable_buffer()
        => ExecutionTestFromFile("resizable-buffer");

    [Fact(DisplayName = "return-abrupt-from-this-out-of-bounds")]
    public Task return_abrupt_from_this_out_of_bounds()
        => ExecutionTestFromFile("return-abrupt-from-this-out-of-bounds");
}
