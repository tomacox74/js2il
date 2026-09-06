using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.indexOf.BigInt;

public class TypedArrayFollowUpConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayFollowUpConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.indexOf.BigInt") { }

    [Fact(DisplayName = "fromIndex-equal-or-greater-length-returns-minus-one.js")]
    public Task fromIndex_equal_or_greater_length_returns_minus_one() => ExecutionTestFromFile("fromIndex-equal-or-greater-length-returns-minus-one");

    [Fact(DisplayName = "fromIndex-infinity.js")]
    public Task fromIndex_infinity() => ExecutionTestFromFile("fromIndex-infinity");

    [Fact(DisplayName = "fromIndex-minus-zero.js")]
    public Task fromIndex_minus_zero() => ExecutionTestFromFile("fromIndex-minus-zero");

    [Fact(DisplayName = "get-length-uses-internal-arraylength.js")]
    public Task get_length_uses_internal_arraylength() => ExecutionTestFromFile("get-length-uses-internal-arraylength");

    [Fact(DisplayName = "no-arg.js")]
    public Task no_arg() => ExecutionTestFromFile("no-arg");

    [Fact(DisplayName = "return-abrupt-tointeger-fromindex-symbol.js")]
    public Task return_abrupt_tointeger_fromindex_symbol() => ExecutionTestFromFile("return-abrupt-tointeger-fromindex-symbol");

    [Fact(DisplayName = "return-abrupt-tointeger-fromindex.js")]
    public Task return_abrupt_tointeger_fromindex() => ExecutionTestFromFile("return-abrupt-tointeger-fromindex");

    [Fact(DisplayName = "search-found-returns-index.js")]
    public Task search_found_returns_index() => ExecutionTestFromFile("search-found-returns-index");

    [Fact(DisplayName = "search-not-found-returns-minus-one.js")]
    public Task search_not_found_returns_minus_one() => ExecutionTestFromFile("search-not-found-returns-minus-one");

    [Fact(DisplayName = "tointeger-fromindex.js")]
    public Task tointeger_fromindex() => ExecutionTestFromFile("tointeger-fromindex");

}
