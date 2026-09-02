using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.of;

public class TypedArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConformanceBatchExecutionTests() : base("built_ins.TypedArray.of") { }

    [Fact(DisplayName = "invoked-as-method.js")]
    public Task invoked_as_method() => ExecutionTestFromFile("invoked-as-method");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "resized-with-out-of-bounds-and-in-bounds-indices.js")]
    public Task resized_with_out_of_bounds_and_in_bounds_indices() => ExecutionTestFromFile("resized-with-out-of-bounds-and-in-bounds-indices");

    [Fact(DisplayName = "this-is-not-constructor.js")]
    public Task this_is_not_constructor() => ExecutionTestFromFile("this-is-not-constructor");

}
