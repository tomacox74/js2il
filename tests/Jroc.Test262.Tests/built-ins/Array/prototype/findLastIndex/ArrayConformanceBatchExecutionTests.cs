using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.findLastIndex;

public class ArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public ArrayConformanceBatchExecutionTests() : base("built_ins.Array.prototype.findLastIndex") { }

    [Fact(DisplayName = "callbackfn-resize-arraybuffer.js")]
    public Task callbackfn_resize_arraybuffer() => ExecutionTestFromFile("callbackfn-resize-arraybuffer");

    [Fact(DisplayName = "predicate-call-this-strict.js")]
    public Task predicate_call_this_strict() => ExecutionTestFromFile("predicate-call-this-strict");

    [Fact(DisplayName = "return-abrupt-from-property.js")]
    public Task return_abrupt_from_property() => ExecutionTestFromFile("return-abrupt-from-property");

    [Fact(DisplayName = "return-abrupt-from-this-length-as-symbol.js")]
    public Task return_abrupt_from_this_length_as_symbol() => ExecutionTestFromFile("return-abrupt-from-this-length-as-symbol");

    [Fact(DisplayName = "return-abrupt-from-this-length.js")]
    public Task return_abrupt_from_this_length() => ExecutionTestFromFile("return-abrupt-from-this-length");

}
