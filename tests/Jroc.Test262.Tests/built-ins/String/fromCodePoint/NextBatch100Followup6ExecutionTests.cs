using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.fromCodePoint;

public class NextBatch100Followup6ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100Followup6ExecutionTests() : base("built_ins.String.fromCodePoint") { }

    [Fact(DisplayName = "argument-is-not-integer")]
    public Task argument_is_not_integer() => ExecutionTestFromFile("argument-is-not-integer");

    [Fact(DisplayName = "arguments-is-empty")]
    public Task arguments_is_empty() => ExecutionTestFromFile("arguments-is-empty");

    [Fact(DisplayName = "fromCodePoint")]
    public Task fromCodePoint() => ExecutionTestFromFile("fromCodePoint");

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "number-is-out-of-range")]
    public Task number_is_out_of_range() => ExecutionTestFromFile("number-is-out-of-range");

    [Fact(DisplayName = "return-string-value")]
    public Task return_string_value() => ExecutionTestFromFile("return-string-value");

    [Fact(DisplayName = "to-number-conversions")]
    public Task to_number_conversions() => ExecutionTestFromFile("to-number-conversions");

}
