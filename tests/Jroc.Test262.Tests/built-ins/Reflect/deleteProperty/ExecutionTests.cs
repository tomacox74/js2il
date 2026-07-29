using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Reflect.deleteProperty;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Reflect.deleteProperty") { }

    [Fact(DisplayName = "delete-properties")]
    public Task delete_properties()
        => ExecutionTestFromFile("delete-properties");
}
