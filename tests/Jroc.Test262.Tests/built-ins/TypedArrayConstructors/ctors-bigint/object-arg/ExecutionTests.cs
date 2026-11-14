using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.ctors_bigint.object_arg;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.TypedArrayConstructors.ctors_bigint.object_arg") { }

    [Fact(DisplayName = "length-excessive-throws.js")]
    public Task Port400_length_excessive_throws()
        => ExecutionTestFromFile("length-excessive-throws");

    [Fact(DisplayName = "undefined-newtarget-throws.js")]
    public Task Port400_undefined_newtarget_throws()
        => ExecutionTestFromFile("undefined-newtarget-throws");
}
