using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.reduce;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.TypedArray.prototype.reduce") { }

    [Fact(DisplayName = "callbackfn-resize")]
    public Task callbackfn_resize()
        => ExecutionTestFromFile("callbackfn-resize");

}
