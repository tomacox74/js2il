using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.ctors.typedarray_arg;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.TypedArrayConstructors.ctors.typedarray_arg") { }

    [Fact(DisplayName = "throw-type-error-before-custom-proto-access.js")]
    public Task Port400_throw_type_error_before_custom_proto_access()
        => ExecutionTestFromFile("throw-type-error-before-custom-proto-access");

    [Fact(DisplayName = "undefined-newtarget-throws.js")]
    public Task Port400_undefined_newtarget_throws()
        => ExecutionTestFromFile("undefined-newtarget-throws");
}
