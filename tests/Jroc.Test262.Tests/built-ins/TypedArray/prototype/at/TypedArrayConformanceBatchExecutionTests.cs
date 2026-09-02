using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.at;

public class TypedArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.at") { }

    [Fact(DisplayName = "index-non-numeric-argument-tointeger.js")]
    public Task index_non_numeric_argument_tointeger() => ExecutionTestFromFile("index-non-numeric-argument-tointeger");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "returns-undefined-for-out-of-range-index.js")]
    public Task returns_undefined_for_out_of_range_index() => ExecutionTestFromFile("returns-undefined-for-out-of-range-index");

}
