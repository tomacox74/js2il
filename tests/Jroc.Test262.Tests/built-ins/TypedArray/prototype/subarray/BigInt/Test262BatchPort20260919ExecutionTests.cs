using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.subarray.BigInt;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.TypedArray.prototype.subarray.BigInt") { }

    [Fact(DisplayName = "tointeger-begin")]
    public Task tointeger_begin()
        => ExecutionTestFromFile("tointeger-begin");

    [Fact(DisplayName = "tointeger-end")]
    public Task tointeger_end()
        => ExecutionTestFromFile("tointeger-end");

}
