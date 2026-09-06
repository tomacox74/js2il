using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.includes;

public class TypedArrayFollowUpConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayFollowUpConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.includes") { }

    [Fact(DisplayName = "fromIndex-infinity.js")]
    public Task fromIndex_infinity() => ExecutionTestFromFile("fromIndex-infinity");

    [Fact(DisplayName = "fromIndex-minus-zero.js")]
    public Task fromIndex_minus_zero() => ExecutionTestFromFile("fromIndex-minus-zero");

    [Fact(DisplayName = "get-length-uses-internal-arraylength.js")]
    public Task get_length_uses_internal_arraylength() => ExecutionTestFromFile("get-length-uses-internal-arraylength");

    [Fact(DisplayName = "index-compared-against-initial-length-out-of-bounds.js")]
    public Task index_compared_against_initial_length_out_of_bounds() => ExecutionTestFromFile("index-compared-against-initial-length-out-of-bounds");

    [Fact(DisplayName = "index-compared-against-initial-length.js")]
    public Task index_compared_against_initial_length() => ExecutionTestFromFile("index-compared-against-initial-length");

    [Fact(DisplayName = "return-abrupt-tointeger-fromindex-symbol.js")]
    public Task return_abrupt_tointeger_fromindex_symbol() => ExecutionTestFromFile("return-abrupt-tointeger-fromindex-symbol");

    [Fact(DisplayName = "return-abrupt-tointeger-fromindex.js")]
    public Task return_abrupt_tointeger_fromindex() => ExecutionTestFromFile("return-abrupt-tointeger-fromindex");

    [Fact(DisplayName = "samevaluezero.js")]
    public Task samevaluezero() => ExecutionTestFromFile("samevaluezero");

    [Fact(DisplayName = "search-found-returns-true.js")]
    public Task search_found_returns_true() => ExecutionTestFromFile("search-found-returns-true");

    [Fact(DisplayName = "search-undefined-after-shrinking-buffer-index-is-oob.js")]
    public Task search_undefined_after_shrinking_buffer_index_is_oob() => ExecutionTestFromFile("search-undefined-after-shrinking-buffer-index-is-oob");

    [Fact(DisplayName = "searchelement-not-integer.js")]
    public Task searchelement_not_integer() => ExecutionTestFromFile("searchelement-not-integer");

    [Fact(DisplayName = "this-is-not-object.js")]
    public Task this_is_not_object() => ExecutionTestFromFile("this-is-not-object");

    [Fact(DisplayName = "this-is-not-typedarray-instance.js")]
    public Task this_is_not_typedarray_instance() => ExecutionTestFromFile("this-is-not-typedarray-instance");

    [Fact(DisplayName = "tointeger-fromindex.js")]
    public Task tointeger_fromindex() => ExecutionTestFromFile("tointeger-fromindex");

}
