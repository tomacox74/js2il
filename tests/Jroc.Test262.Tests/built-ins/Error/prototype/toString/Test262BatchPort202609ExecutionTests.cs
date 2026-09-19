using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Error.prototype.toString;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Error.prototype.toString") { }

    [Fact(DisplayName = "15.11.4.4-10-1")]
    public Task _15_11_4_4_10_1()
        => ExecutionTestFromFile("15.11.4.4-10-1");

    [Fact(DisplayName = "15.11.4.4-6-1")]
    public Task _15_11_4_4_6_1()
        => ExecutionTestFromFile("15.11.4.4-6-1");

    [Fact(DisplayName = "15.11.4.4-6-2")]
    public Task _15_11_4_4_6_2()
        => ExecutionTestFromFile("15.11.4.4-6-2");

    [Fact(DisplayName = "15.11.4.4-8-1")]
    public Task _15_11_4_4_8_1()
        => ExecutionTestFromFile("15.11.4.4-8-1");

    [Fact(DisplayName = "15.11.4.4-8-2")]
    public Task _15_11_4_4_8_2()
        => ExecutionTestFromFile("15.11.4.4-8-2");

    [Fact(DisplayName = "15.11.4.4-9-1")]
    public Task _15_11_4_4_9_1()
        => ExecutionTestFromFile("15.11.4.4-9-1");

    [Fact(DisplayName = "S15.11.4.4_A2")]
    public Task S15_11_4_4_A2()
        => ExecutionTestFromFile("S15.11.4.4_A2");

    [Fact(DisplayName = "called-as-function")]
    public Task called_as_function()
        => ExecutionTestFromFile("called-as-function");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "tostring-get-throws")]
    public Task tostring_get_throws()
        => ExecutionTestFromFile("tostring-get-throws");

    [Fact(DisplayName = "tostring-message-throws-symbol")]
    public Task tostring_message_throws_symbol()
        => ExecutionTestFromFile("tostring-message-throws-symbol");

    [Fact(DisplayName = "tostring-message-throws-toprimitive")]
    public Task tostring_message_throws_toprimitive()
        => ExecutionTestFromFile("tostring-message-throws-toprimitive");

    [Fact(DisplayName = "undefined-props")]
    public Task undefined_props()
        => ExecutionTestFromFile("undefined-props");

}
