using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.Symbol_toStringTag.BigInt;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.TypedArray.prototype.Symbol_toStringTag.BigInt") { }

    [Fact(DisplayName = "return-typedarrayname")]
    public Task return_typedarrayname()
        => ExecutionTestFromFile("return-typedarrayname");
}
