using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.entries.BigInt;

public class TypedArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.entries.BigInt") { }

    [Fact(DisplayName = "iter-prototype.js")]
    public Task iter_prototype() => ExecutionTestFromFile("iter-prototype");

    [Fact(DisplayName = "return-itor.js")]
    public Task return_itor() => ExecutionTestFromFile("return-itor");

}
