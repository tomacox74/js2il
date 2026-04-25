using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.AsyncFunction;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncFunction") { }

    [Fact(DisplayName = "AsyncFunction_intrinsic")]
    public Task AsyncFunction_intrinsic()
        => ExecutionTest("AsyncFunction_intrinsic");
}
