using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Reflect.preventExtensions;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Reflect.preventExtensions") { }

    [Fact(DisplayName = "prevent-extensions")]
    public Task prevent_extensions()
        => ExecutionTestFromFile("prevent-extensions");
}
