namespace Jroc.Test262.Tests.built_ins.Object.prototype.toLocaleString;

public class ObjectCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public ObjectCompletionExecutionTests() : base("built_ins.Object.prototype.toLocaleString") { }

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");
}
