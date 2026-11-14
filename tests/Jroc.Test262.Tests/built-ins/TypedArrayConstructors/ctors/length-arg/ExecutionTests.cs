using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.ctors.length_arg;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.TypedArrayConstructors.ctors.length_arg") { }

    [Fact(DisplayName = "is-symbol-throws.js")]
    public Task Port400_is_symbol_throws()
        => ExecutionTestFromFile("is-symbol-throws");

    [Fact(DisplayName = "toindex-length.js")]
    public Task Port400_toindex_length()
        => ExecutionTestFromFile("toindex-length");

    [Fact(DisplayName = "undefined-newtarget-throws.js")]
    public Task Port400_undefined_newtarget_throws()
        => ExecutionTestFromFile("undefined-newtarget-throws");
}
