using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.BigInt.prototype.valueOf;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.BigInt.prototype.valueOf") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "this-value-invalid-primitive-throws")]
    public Task this_value_invalid_primitive_throws()
        => ExecutionTestFromFile("this-value-invalid-primitive-throws");

}
