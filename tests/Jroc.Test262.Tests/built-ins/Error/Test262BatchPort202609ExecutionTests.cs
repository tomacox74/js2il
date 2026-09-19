using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Error;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Error") { }

    [Fact(DisplayName = "cause_abrupt")]
    public Task cause_abrupt()
        => ExecutionTestFromFile("cause_abrupt");

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "instance-prototype")]
    public Task instance_prototype()
        => ExecutionTestFromFile("instance-prototype");

    [Fact(DisplayName = "is-a-constructor")]
    public Task is_a_constructor()
        => ExecutionTestFromFile("is-a-constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "the-initial-value-of-errorprototypemessage-is-the-empty-string")]
    public Task the_initial_value_of_errorprototypemessage_is_the_empty_string()
        => ExecutionTestFromFile("the-initial-value-of-errorprototypemessage-is-the-empty-string");

}
