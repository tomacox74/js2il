using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.BigInt;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.BigInt") { }

    [Fact(DisplayName = "call-value-of-when-to-string-present")]
    public Task call_value_of_when_to_string_present()
        => ExecutionTestFromFile("call-value-of-when-to-string-present");

    [Fact(DisplayName = "constructor-integer")]
    public Task constructor_integer()
        => ExecutionTestFromFile("constructor-integer");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto")]
    public Task proto()
        => ExecutionTestFromFile("proto");

    [Fact(DisplayName = "valueof-throws")]
    public Task valueof_throws()
        => ExecutionTestFromFile("valueof-throws");

}
