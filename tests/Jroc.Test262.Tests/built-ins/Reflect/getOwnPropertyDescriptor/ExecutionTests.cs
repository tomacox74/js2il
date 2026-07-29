using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Reflect.getOwnPropertyDescriptor;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Reflect.getOwnPropertyDescriptor") { }

    [Fact(DisplayName = "return-from-data-descriptor")]
    public Task return_from_data_descriptor()
        => ExecutionTestFromFile("return-from-data-descriptor");
}
