using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView.prototype.getFloat16;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.DataView.prototype.getFloat16") { }

    [Fact(DisplayName = "detached-buffer-before-outofrange-byteoffset.js")]
    public Task Port400_detached_buffer_before_outofrange_byteoffset()
        => ExecutionTestFromFile("detached-buffer-before-outofrange-byteoffset");

    [Fact(DisplayName = "detached-buffer.js")]
    public Task Port400_detached_buffer()
        => ExecutionTestFromFile("detached-buffer");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port400_not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");
}
