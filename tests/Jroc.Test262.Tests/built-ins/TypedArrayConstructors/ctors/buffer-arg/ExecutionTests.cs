using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.ctors.buffer_arg;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.built-ins.TypedArrayConstructors.ctors.buffer-arg") { }

    [Fact(DisplayName = "byteoffset-to-number-detachbuffer.js")]
    public Task byteoffset_to_number_detachbuffer() => ExecutionTestFromFile("byteoffset-to-number-detachbuffer");

    [Fact(DisplayName = "detachedbuffer.js")]
    public Task detachedbuffer() => ExecutionTestFromFile("detachedbuffer");

    [Fact(DisplayName = "length-to-number-detachbuffer.js")]
    public Task length_to_number_detachbuffer() => ExecutionTestFromFile("length-to-number-detachbuffer");

    [Fact(DisplayName = "resizable-out-of-bounds.js")]
    public Task resizable_out_of_bounds() => ExecutionTestFromFile("resizable-out-of-bounds");

    [Fact(DisplayName = "toindex-bytelength.js")]
    public Task toindex_bytelength() => ExecutionTestFromFile("toindex-bytelength");

    [Fact(DisplayName = "toindex-byteoffset.js")]
    public Task toindex_byteoffset() => ExecutionTestFromFile("toindex-byteoffset");
}
