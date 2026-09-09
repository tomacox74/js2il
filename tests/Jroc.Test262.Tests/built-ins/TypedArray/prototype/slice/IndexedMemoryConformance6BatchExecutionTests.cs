using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.slice;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.prototype.slice") { }

    [Fact(DisplayName = "bit-precision.js")]
    public Task bit_precision() => ExecutionTestFromFile("bit-precision");

    [Fact(DisplayName = "return-abrupt-from-end-symbol.js")]
    public Task return_abrupt_from_end_symbol() => ExecutionTestFromFile("return-abrupt-from-end-symbol");

    [Fact(DisplayName = "return-abrupt-from-start-symbol.js")]
    public Task return_abrupt_from_start_symbol() => ExecutionTestFromFile("return-abrupt-from-start-symbol");

    [Fact(DisplayName = "this-is-not-object.js")]
    public Task this_is_not_object() => ExecutionTestFromFile("this-is-not-object");

    [Fact(DisplayName = "this-is-not-typedarray-instance.js")]
    public Task this_is_not_typedarray_instance() => ExecutionTestFromFile("this-is-not-typedarray-instance");
}
