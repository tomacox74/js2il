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

    [Fact(DisplayName = "invoked-with-undefined-newtarget-sab.js")]
    public Task Port400_invoked_with_undefined_newtarget_sab()
        => ExecutionTestFromFile("invoked-with-undefined-newtarget-sab");

    [Fact(DisplayName = "invoked-with-undefined-newtarget.js")]
    public Task Port400_invoked_with_undefined_newtarget()
        => ExecutionTestFromFile("invoked-with-undefined-newtarget");

    [Fact(DisplayName = "toindex-bytelength-sab.js")]
    public Task Port400_toindex_bytelength_sab()
        => ExecutionTestFromFile("toindex-bytelength-sab");

    [Fact(DisplayName = "toindex-byteoffset-sab.js")]
    public Task Port400_toindex_byteoffset_sab()
        => ExecutionTestFromFile("toindex-byteoffset-sab");
}
