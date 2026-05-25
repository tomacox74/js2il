using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Object.setPrototypeOf;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.setPrototypeOf") { }

    [Fact(DisplayName = "success")]
    public Task success()
        => ExecutionTestFromFile("success");
    [Fact(DisplayName = "proto-not-obj")]
    public Task proto_not_obj()
        => ExecutionTestFromFile("proto-not-obj");
    [Fact(DisplayName = "o-not-obj-coercible")]
    public Task o_not_obj_coercible()
        => ExecutionTestFromFile("o-not-obj-coercible");
}
