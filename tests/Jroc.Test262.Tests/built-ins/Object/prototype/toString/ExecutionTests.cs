using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.prototype.toString;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.prototype.toString") { }

    [Fact(DisplayName = "Object.prototype.toString.call-bigint")]
    public Task Object_prototype_toString_call_bigint()
        => ExecutionTestFromFile("Object.prototype.toString.call-bigint");
}
