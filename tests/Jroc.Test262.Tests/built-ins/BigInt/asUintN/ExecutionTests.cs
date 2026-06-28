using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.BigInt.asUintN;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.BigInt.asUintN") { }

    [Fact(DisplayName = "arithmetic")]
    public Task arithmetic()
        => ExecutionTestFromFile("arithmetic");

    [Fact(DisplayName = "asUintN")]
    public Task asUintN()
        => ExecutionTestFromFile("asUintN");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");
}
