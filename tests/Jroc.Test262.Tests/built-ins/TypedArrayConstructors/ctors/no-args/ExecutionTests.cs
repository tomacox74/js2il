using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.ctors.no_args;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.TypedArrayConstructors.ctors.no_args") { }

    [Fact(DisplayName = "undefined-newtarget-throws.js")]
    public Task Port400_undefined_newtarget_throws()
        => ExecutionTestFromFile("undefined-newtarget-throws");
}
