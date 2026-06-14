using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.setPrototypeOf;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.setPrototypeOf") { }

    [Fact(DisplayName = "o-not-obj-coercible")]
    public Task o_not_obj_coercible()
        => ExecutionTestFromFile("o-not-obj-coercible");
    [Fact(DisplayName = "property-descriptor")]
    public Task property_descriptor()
        => ExecutionTestFromFile("property-descriptor");
    [Fact(DisplayName = "proto-not-obj")]
    public Task proto_not_obj()
        => ExecutionTestFromFile("proto-not-obj");
    [Fact(DisplayName = "success")]
    public Task success()
        => ExecutionTestFromFile("success");
}
