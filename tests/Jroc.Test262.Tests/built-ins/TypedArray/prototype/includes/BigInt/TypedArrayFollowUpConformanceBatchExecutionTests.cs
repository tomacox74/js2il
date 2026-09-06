using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.includes.BigInt;

public class TypedArrayFollowUpConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayFollowUpConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.includes.BigInt") { }

    [Fact(DisplayName = "fromIndex-equal-or-greater-length-returns-false.js")]
    public Task fromIndex_equal_or_greater_length_returns_false() => ExecutionTestFromFile("fromIndex-equal-or-greater-length-returns-false");

    [Fact(DisplayName = "fromIndex-infinity.js")]
    public Task fromIndex_infinity() => ExecutionTestFromFile("fromIndex-infinity");

    [Fact(DisplayName = "fromIndex-minus-zero.js")]
    public Task fromIndex_minus_zero() => ExecutionTestFromFile("fromIndex-minus-zero");

    [Fact(DisplayName = "get-length-uses-internal-arraylength.js")]
    public Task get_length_uses_internal_arraylength() => ExecutionTestFromFile("get-length-uses-internal-arraylength");

    [Fact(DisplayName = "return-abrupt-tointeger-fromindex-symbol.js")]
    public Task return_abrupt_tointeger_fromindex_symbol() => ExecutionTestFromFile("return-abrupt-tointeger-fromindex-symbol");

    [Fact(DisplayName = "return-abrupt-tointeger-fromindex.js")]
    public Task return_abrupt_tointeger_fromindex() => ExecutionTestFromFile("return-abrupt-tointeger-fromindex");

    [Fact(DisplayName = "search-found-returns-true.js")]
    public Task search_found_returns_true() => ExecutionTestFromFile("search-found-returns-true");

    [Fact(DisplayName = "search-not-found-returns-false.js")]
    public Task search_not_found_returns_false() => ExecutionTestFromFile("search-not-found-returns-false");

    [Fact(DisplayName = "tointeger-fromindex.js")]
    public Task tointeger_fromindex() => ExecutionTestFromFile("tointeger-fromindex");

}
