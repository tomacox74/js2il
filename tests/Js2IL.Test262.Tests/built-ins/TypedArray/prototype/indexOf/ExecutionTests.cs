using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.TypedArray.prototype.indexOf;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("TypedArray.prototype.indexOf") { }

    [Fact(DisplayName = "fromIndex-equal-or-greater-length-returns-minus-one")]
    public Task fromIndex_equal_or_greater_length_returns_minus_one()
        => ExecutionTestFromFile("fromIndex-equal-or-greater-length-returns-minus-one");

    [Fact(DisplayName = "search-not-found-returns-minus-one")]
    public Task search_not_found_returns_minus_one()
        => ExecutionTestFromFile("search-not-found-returns-minus-one");
}
