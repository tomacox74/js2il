using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.BigInt.asIntN;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.BigInt.asIntN") { }

    [Fact(DisplayName = "arithmetic")]
    public Task arithmetic()
        => ExecutionTestFromFile("arithmetic");

    [Fact(DisplayName = "asIntN")]
    public Task asIntN()
        => ExecutionTestFromFile("asIntN");

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
