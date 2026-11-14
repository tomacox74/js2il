using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.ctors_bigint.buffer_arg;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.TypedArrayConstructors.ctors_bigint.buffer_arg") { }

    [Fact(DisplayName = "byteoffset-to-number-detachbuffer.js")]
    public Task Port400_byteoffset_to_number_detachbuffer()
        => ExecutionTestFromFile("byteoffset-to-number-detachbuffer");

    [Fact(DisplayName = "detachedbuffer.js")]
    public Task Port400_detachedbuffer()
        => ExecutionTestFromFile("detachedbuffer");

    [Fact(DisplayName = "invoked-with-undefined-newtarget-sab.js")]
    public Task Port400_invoked_with_undefined_newtarget_sab()
        => ExecutionTestFromFile("invoked-with-undefined-newtarget-sab");

    [Fact(DisplayName = "invoked-with-undefined-newtarget.js")]
    public Task Port400_invoked_with_undefined_newtarget()
        => ExecutionTestFromFile("invoked-with-undefined-newtarget");

    [Fact(DisplayName = "length-to-number-detachbuffer.js")]
    public Task Port400_length_to_number_detachbuffer()
        => ExecutionTestFromFile("length-to-number-detachbuffer");

    [Fact(DisplayName = "toindex-bytelength-sab.js")]
    public Task Port400_toindex_bytelength_sab()
        => ExecutionTestFromFile("toindex-bytelength-sab");

    [Fact(DisplayName = "toindex-bytelength.js")]
    public Task Port400_toindex_bytelength()
        => ExecutionTestFromFile("toindex-bytelength");

    [Fact(DisplayName = "toindex-byteoffset-sab.js")]
    public Task Port400_toindex_byteoffset_sab()
        => ExecutionTestFromFile("toindex-byteoffset-sab");

    [Fact(DisplayName = "toindex-byteoffset.js")]
    public Task Port400_toindex_byteoffset()
        => ExecutionTestFromFile("toindex-byteoffset");
}
