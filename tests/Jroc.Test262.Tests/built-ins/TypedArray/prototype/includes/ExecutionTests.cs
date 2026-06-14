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
}
