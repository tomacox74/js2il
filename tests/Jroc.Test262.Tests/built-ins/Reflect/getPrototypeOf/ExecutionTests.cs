using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Reflect.getPrototypeOf;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Reflect.getPrototypeOf") { }

    [Fact(DisplayName = "return-prototype")]
    public Task return_prototype()
        => ExecutionTestFromFile("return-prototype");
}
