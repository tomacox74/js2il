using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Reflect.isExtensible;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Reflect.isExtensible") { }

    [Fact(DisplayName = "return-boolean")]
    public Task return_boolean()
        => ExecutionTestFromFile("return-boolean");
}
