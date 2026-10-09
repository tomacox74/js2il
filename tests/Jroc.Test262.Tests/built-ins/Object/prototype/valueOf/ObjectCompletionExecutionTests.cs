namespace Jroc.Test262.Tests.built_ins.Object.prototype.valueOf;

public class ObjectCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public ObjectCompletionExecutionTests() : base("built_ins.Object.prototype.valueOf") { }

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");
}
