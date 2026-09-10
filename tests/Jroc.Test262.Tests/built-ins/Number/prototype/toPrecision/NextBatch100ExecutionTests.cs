using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Number.prototype.toPrecision;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.Number.prototype.toPrecision") { }

    [Fact(DisplayName = "exponential")]
    public Task exponential() => ExecutionTestFromFile("exponential");

    [Fact(DisplayName = "infinity")]
    public Task infinity() => ExecutionTestFromFile("infinity");

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "range")]
    public Task range() => ExecutionTestFromFile("range");

    [Fact(DisplayName = "return-abrupt-tointeger-precision-symbol")]
    public Task return_abrupt_tointeger_precision_symbol() => ExecutionTestFromFile("return-abrupt-tointeger-precision-symbol");

    [Fact(DisplayName = "return-abrupt-tointeger-precision")]
    public Task return_abrupt_tointeger_precision() => ExecutionTestFromFile("return-abrupt-tointeger-precision");

    [Fact(DisplayName = "this-type-not-number-or-number-object")]
    public Task this_type_not_number_or_number_object() => ExecutionTestFromFile("this-type-not-number-or-number-object");

    [Fact(DisplayName = "tointeger-precision")]
    public Task tointeger_precision() => ExecutionTestFromFile("tointeger-precision");

    [Fact(DisplayName = "undefined-precision-arg")]
    public Task undefined_precision_arg() => ExecutionTestFromFile("undefined-precision-arg");
}
