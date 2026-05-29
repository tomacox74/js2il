using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Array.prototype.includes;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.includes") { }

    [Fact(DisplayName = "fromIndex-equal-or-greater-length-returns-false")]
    public Task fromIndex_equal_or_greater_length_returns_false()
        => ExecutionTestFromFile("fromIndex-equal-or-greater-length-returns-false");

    [Fact(DisplayName = "fromIndex-infinity")]
    public Task fromIndex_infinity()
        => ExecutionTestFromFile("fromIndex-infinity");

    [Fact(DisplayName = "fromIndex-minus-zero")]
    public Task fromIndex_minus_zero()
        => ExecutionTestFromFile("fromIndex-minus-zero");

    [Fact(DisplayName = "length-zero-returns-false")]
    public Task length_zero_returns_false()
        => ExecutionTestFromFile("length-zero-returns-false");

    [Fact(DisplayName = "search-found-returns-true")]
    public Task search_found_returns_true()
        => ExecutionTestFromFile("search-found-returns-true");

    [Fact(DisplayName = "search-not-found-returns-false")]
    public Task search_not_found_returns_false()
        => ExecutionTestFromFile("search-not-found-returns-false");

    [Fact(DisplayName = "sparse")]
    public Task sparse()
        => ExecutionTestFromFile("sparse");

    [Fact(DisplayName = "using-fromindex")]
    public Task using_fromindex()
        => ExecutionTestFromFile("using-fromindex");

}
