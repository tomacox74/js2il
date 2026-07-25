using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.SharedArrayBuffer.prototype.slice;

public sealed class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.SharedArrayBuffer.prototype.slice") { }

    [Fact(DisplayName = "context-is-not-arraybuffer-object")]
    public Task context_is_not_arraybuffer_object()
        => ExecutionTest("context-is-not-arraybuffer-object");

    [Fact(DisplayName = "context-is-not-object")]
    public Task context_is_not_object()
        => ExecutionTest("context-is-not-object");

}
