namespace Jroc.Test262.Tests.built_ins.Object.entries;

public partial class ExecutionTests
{
    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
}
