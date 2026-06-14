using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.from;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("TypedArray.from") { }

        [Fact(DisplayName = "invoked-as-method")]
    public Task invoked_as_method()
        => ExecutionTestFromFile("invoked-as-method");

    [Fact(DisplayName = "mapfn-is-not-callable")]
    public Task mapfn_is_not_callable()
        => ExecutionTestFromFile("mapfn-is-not-callable");

    [Fact(DisplayName = "this-is-not-constructor")]
    public Task this_is_not_constructor()
        => ExecutionTestFromFile("this-is-not-constructor");
}
