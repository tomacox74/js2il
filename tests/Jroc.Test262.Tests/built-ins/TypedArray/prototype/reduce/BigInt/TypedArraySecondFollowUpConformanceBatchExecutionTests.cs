using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.reduce.BigInt;

public class TypedArraySecondFollowUpConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArraySecondFollowUpConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.reduce.BigInt") { }

    [Fact(DisplayName = "return-first-value-without-callbackfn.js")]
    public Task return_first_value_without_callbackfn() => ExecutionTestFromFile("return-first-value-without-callbackfn");

}
