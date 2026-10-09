namespace Jroc.Test262.Tests.built_ins.Object.prototype.__defineSetter__;

public class ObjectCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public ObjectCompletionExecutionTests() : base("built_ins.Object.prototype.__defineSetter__") { }

    [Fact(DisplayName = "define-abrupt.js")]
    public Task define_abrupt() => ExecutionTestFromFile("define-abrupt");
}
