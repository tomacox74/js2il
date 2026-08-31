using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.includes;

public partial class ExecutionTests : InMemoryExecutionTestsBase
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

    [Fact(DisplayName = "call-with-boolean")]
    public Task call_with_boolean()
        => ExecutionTestFromFile("call-with-boolean");

    [Fact(DisplayName = "get-prop")]
    public Task get_prop()
        => ExecutionTestFromFile("get-prop");

    [Fact(DisplayName = "length-boundaries")]
    public Task length_boundaries()
        => ExecutionTestFromFile("length-boundaries");

    [Fact(DisplayName = "return-abrupt-get-length")]
    public Task return_abrupt_get_length()
        => ExecutionTestFromFile("return-abrupt-get-length");

    [Fact(DisplayName = "return-abrupt-get-prop")]
    public Task return_abrupt_get_prop()
        => ExecutionTestFromFile("return-abrupt-get-prop");

    [Fact(DisplayName = "return-abrupt-tointeger-fromindex")]
    public Task return_abrupt_tointeger_fromindex()
        => ExecutionTestFromFile("return-abrupt-tointeger-fromindex");

    [Fact(DisplayName = "return-abrupt-tonumber-length")]
    public Task return_abrupt_tonumber_length()
        => ExecutionTestFromFile("return-abrupt-tonumber-length");

}
