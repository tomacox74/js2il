using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.every.BigInt;

public class FailingBatchExecutionTests : DiskExecutionTestsBase
{
    public FailingBatchExecutionTests() : base("built_ins.TypedArray.prototype.every.BigInt") { }

    [Fact(DisplayName = "callbackfn-detachbuffer.js")]
    public Task callbackfn_detachbuffer() => ExecutionTestFromFile("callbackfn-detachbuffer");

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

    [Fact(DisplayName = "returns-true-if-every-cb-returns-true.js")]
    public Task returns_true_if_every_cb_returns_true() => ExecutionTestFromFile("returns-true-if-every-cb-returns-true");
}
