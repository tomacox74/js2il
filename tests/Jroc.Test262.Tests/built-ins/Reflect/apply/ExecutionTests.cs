using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Reflect.apply;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Reflect.apply") { }

    [Fact(DisplayName = "call-target")]
    public Task call_target()
        => ExecutionTestFromFile("call-target");
}
