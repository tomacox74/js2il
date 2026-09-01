namespace Jroc.Test262.Tests.built_ins.Object.@is;

public partial class ExecutionTests
{
    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "object-is.js")]
    public Task object_is() => ExecutionTestFromFile("object-is");
}
