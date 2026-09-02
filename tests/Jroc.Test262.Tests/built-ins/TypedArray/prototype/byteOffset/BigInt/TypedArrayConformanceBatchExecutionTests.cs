using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.byteOffset.BigInt;

public class TypedArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.byteOffset.BigInt") { }

    [Fact(DisplayName = "resizable-array-buffer-auto.js")]
    public Task resizable_array_buffer_auto() => ExecutionTestFromFile("resizable-array-buffer-auto");

    [Fact(DisplayName = "resizable-array-buffer-fixed.js")]
    public Task resizable_array_buffer_fixed() => ExecutionTestFromFile("resizable-array-buffer-fixed");

    [Fact(DisplayName = "return-byteoffset.js")]
    public Task return_byteoffset() => ExecutionTestFromFile("return-byteoffset");

}
