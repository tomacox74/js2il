namespace Jroc.Test262.Tests.built_ins.Object.prototype.__defineGetter__;

public class ObjectCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public ObjectCompletionExecutionTests() : base("built_ins.Object.prototype.__defineGetter__") { }

    [Fact(DisplayName = "define-abrupt.js")]
    public Task define_abrupt() => ExecutionTestFromFile("define-abrupt");
}
