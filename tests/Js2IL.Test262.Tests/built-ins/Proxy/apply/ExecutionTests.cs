using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Proxy.apply;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Proxy.apply") { }

    [Fact(DisplayName = "trap-is-undefined")]
    public Task trap_is_undefined()
        => ExecutionTestFromFile("trap-is-undefined");
}
