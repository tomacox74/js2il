using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Number.prototype.toExponential;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.Number.prototype.toExponential") { }

    [Fact(DisplayName = "infinity")]
    public Task infinity() => ExecutionTestFromFile("infinity");

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "nan")]
    public Task nan() => ExecutionTestFromFile("nan");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "range")]
    public Task range() => ExecutionTestFromFile("range");

    [Fact(DisplayName = "this-type-not-number-or-number-object")]
    public Task this_type_not_number_or_number_object() => ExecutionTestFromFile("this-type-not-number-or-number-object");

    [Fact(DisplayName = "tointeger-fractiondigits")]
    public Task tointeger_fractiondigits() => ExecutionTestFromFile("tointeger-fractiondigits");
}
