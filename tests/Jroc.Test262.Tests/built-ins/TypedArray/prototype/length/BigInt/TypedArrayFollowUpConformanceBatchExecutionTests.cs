using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.length.BigInt;

public class TypedArrayFollowUpConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayFollowUpConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.length.BigInt") { }

    [Fact(DisplayName = "resizable-array-buffer-auto.js")]
    public Task resizable_array_buffer_auto() => ExecutionTestFromFile("resizable-array-buffer-auto");

    [Fact(DisplayName = "resizable-array-buffer-fixed.js")]
    public Task resizable_array_buffer_fixed() => ExecutionTestFromFile("resizable-array-buffer-fixed");

    [Fact(DisplayName = "return-length.js")]
    public Task return_length() => ExecutionTestFromFile("return-length");

}
