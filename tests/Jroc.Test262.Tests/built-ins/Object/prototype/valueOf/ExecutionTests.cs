namespace Jroc.Test262.Tests.built_ins.Object.prototype.valueOf;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.prototype.valueOf") { }

    [Fact(DisplayName = "built-ins/Object/prototype/valueOf/15.2.4.4-1.js")]
    public Task test_15_2_4_4_1() => ExecutionTestFromFile("15.2.4.4-1");

    [Fact(DisplayName = "built-ins/Object/prototype/valueOf/15.2.4.4-2.js")]
    public Task test_15_2_4_4_2() => ExecutionTestFromFile("15.2.4.4-2");
}
