using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.keys.BigInt;

public class TypedArrayFollowUpConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayFollowUpConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.keys.BigInt") { }

    [Fact(DisplayName = "iter-prototype.js")]
    public Task iter_prototype() => ExecutionTestFromFile("iter-prototype");

    [Fact(DisplayName = "return-itor.js")]
    public Task return_itor() => ExecutionTestFromFile("return-itor");

}
