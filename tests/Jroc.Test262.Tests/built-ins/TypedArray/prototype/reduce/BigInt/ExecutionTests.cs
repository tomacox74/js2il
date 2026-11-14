using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.reduce.BigInt;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.TypedArray.prototype.reduce.BigInt") { }

    [Fact(DisplayName = "callbackfn-detachbuffer.js")]
    public Task Port400_callbackfn_detachbuffer()
        => ExecutionTestFromFile("callbackfn-detachbuffer");
}
