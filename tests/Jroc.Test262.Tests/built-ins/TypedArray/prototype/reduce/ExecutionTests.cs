using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.reduce;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.built-ins.TypedArray.prototype.reduce") { }

    [Fact(DisplayName = "resizable-buffer-grow-mid-iteration.js")]
    public Task resizable_buffer_grow_mid_iteration() => ExecutionTestFromFile("resizable-buffer-grow-mid-iteration");

    [Fact(DisplayName = "resizable-buffer-shrink-mid-iteration.js")]
    public Task resizable_buffer_shrink_mid_iteration() => ExecutionTestFromFile("resizable-buffer-shrink-mid-iteration");

    [Fact(DisplayName = "resizable-buffer.js")]
    public Task resizable_buffer() => ExecutionTestFromFile("resizable-buffer");


    [Fact(DisplayName = "callbackfn-detachbuffer.js")]
    public Task Port400_callbackfn_detachbuffer()
        => ExecutionTestFromFile("callbackfn-detachbuffer");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port400_not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");
}
