using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Reflect.get;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Reflect.get") { }

    [Fact(DisplayName = "return-value")]
    public Task return_value()
        => ExecutionTestFromFile("return-value");

    [Fact(DisplayName = "return-value-from-receiver")]
    public Task return_value_from_receiver()
        => ExecutionTestFromFile("return-value-from-receiver");
}
