using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.forEach;

public class ArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public ArrayConformanceBatchExecutionTests() : base("built_ins.Array.prototype.forEach") { }

    [Fact(DisplayName = "callbackfn-resize-arraybuffer.js")]
    public Task callbackfn_resize_arraybuffer() => ExecutionTestFromFile("callbackfn-resize-arraybuffer");

}
