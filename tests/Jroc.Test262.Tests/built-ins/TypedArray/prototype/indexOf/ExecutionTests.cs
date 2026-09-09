using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.indexOf;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("TypedArray.prototype.indexOf") { }

    [Fact(DisplayName = "fromIndex-equal-or-greater-length-returns-minus-one")]
    public Task fromIndex_equal_or_greater_length_returns_minus_one()
        => ExecutionTestFromFile("fromIndex-equal-or-greater-length-returns-minus-one");

    [Fact(DisplayName = "search-not-found-returns-minus-one")]
    public Task search_not_found_returns_minus_one()
        => ExecutionTestFromFile("search-not-found-returns-minus-one");

    [Fact(DisplayName = "detached-buffer-during-fromIndex-returns-minus-one-for-undefined.js")]
    public Task detached_buffer_during_fromIndex_returns_minus_one_for_undefined() => ExecutionTestFromFile("detached-buffer-during-fromIndex-returns-minus-one-for-undefined");
    [Fact(DisplayName = "detached-buffer-during-fromIndex-returns-minus-one-for-zero.js")]
    public Task detached_buffer_during_fromIndex_returns_minus_one_for_zero() => ExecutionTestFromFile("detached-buffer-during-fromIndex-returns-minus-one-for-zero");
    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");
    [Fact(DisplayName = "resizable-buffer-special-float-values.js")]
    public Task resizable_buffer_special_float_values() => ExecutionTestFromFile("resizable-buffer-special-float-values");
    [Fact(DisplayName = "resizable-buffer.js")]
    public Task resizable_buffer() => ExecutionTestFromFile("resizable-buffer");
}
