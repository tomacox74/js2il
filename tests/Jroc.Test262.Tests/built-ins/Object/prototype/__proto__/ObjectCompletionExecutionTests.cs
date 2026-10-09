namespace Jroc.Test262.Tests.built_ins.Object.prototype.__proto__;

public class ObjectCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public ObjectCompletionExecutionTests() : base("built_ins.Object.prototype.__proto__") { }

    [Fact(DisplayName = "set-abrupt.js")]
    public Task set_abrupt() => ExecutionTestFromFile("set-abrupt");
}
