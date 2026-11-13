using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.groupBy;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.groupBy") { }

    [Fact(DisplayName = "callback-arg")]
    public Task callback_arg() => ExecutionTestFromFile("callback-arg");
    [Fact(DisplayName = "emptyList")]
    public Task emptyList() => ExecutionTestFromFile("emptyList");
    [Fact(DisplayName = "invalid-callback")]
    public Task invalid_callback() => ExecutionTestFromFile("invalid-callback");
    [Fact(DisplayName = "invalid-property-key")]
    public Task invalid_property_key() => ExecutionTestFromFile("invalid-property-key");
    [Fact(DisplayName = "iterator-next-throws")]
    public Task iterator_next_throws() => ExecutionTestFromFile("iterator-next-throws");
    [Fact(DisplayName = "null-prototype")]
    public Task null_prototype() => ExecutionTestFromFile("null-prototype");
    [Fact(DisplayName = "toPropertyKey")]
    public Task toPropertyKey() => ExecutionTestFromFile("toPropertyKey");

    [Fact(DisplayName = "callback-throws.js")]
    public Task Port200_callback_throws()
        => ExecutionTestFromFile("callback-throws");

    [Fact(DisplayName = "evenOdd.js")]
    public Task Port200_evenOdd()
        => ExecutionTestFromFile("evenOdd");

    [Fact(DisplayName = "groupLength.js")]
    public Task Port200_groupLength()
        => ExecutionTestFromFile("groupLength");

    [Fact(DisplayName = "invalid-iterable.js")]
    public Task Port200_invalid_iterable()
        => ExecutionTestFromFile("invalid-iterable");

    [Fact(DisplayName = "length.js")]
    public Task Port200_length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task Port200_name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "string.js")]
    public Task Port200_string()
        => ExecutionTestFromFile("string");
}
