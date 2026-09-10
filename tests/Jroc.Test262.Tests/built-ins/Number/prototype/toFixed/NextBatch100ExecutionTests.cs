using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Number.prototype.toFixed;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.Number.prototype.toFixed") { }

    [Fact(DisplayName = "S15.7.4.5_A1.1_T01")]
    public Task S15_7_4_5_A1_1_T01() => ExecutionTestFromFile("S15.7.4.5_A1.1_T01");

    [Fact(DisplayName = "S15.7.4.5_A1.1_T02")]
    public Task S15_7_4_5_A1_1_T02() => ExecutionTestFromFile("S15.7.4.5_A1.1_T02");

    [Fact(DisplayName = "S15.7.4.5_A1.3_T02")]
    public Task S15_7_4_5_A1_3_T02() => ExecutionTestFromFile("S15.7.4.5_A1.3_T02");

    [Fact(DisplayName = "S15.7.4.5_A2_T01")]
    public Task S15_7_4_5_A2_T01() => ExecutionTestFromFile("S15.7.4.5_A2_T01");

    [Fact(DisplayName = "exactness")]
    public Task exactness() => ExecutionTestFromFile("exactness");

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "range")]
    public Task range() => ExecutionTestFromFile("range");

    [Fact(DisplayName = "toFixed-tonumber-throws-typeerror-bigint")]
    public Task toFixed_tonumber_throws_typeerror_bigint() => ExecutionTestFromFile("toFixed-tonumber-throws-typeerror-bigint");

    [Fact(DisplayName = "toFixed-tonumber-throws-typeerror-symbol")]
    public Task toFixed_tonumber_throws_typeerror_symbol() => ExecutionTestFromFile("toFixed-tonumber-throws-typeerror-symbol");

    [Fact(DisplayName = "toFixed-tonumber-throws-typeerror-toprimitive")]
    public Task toFixed_tonumber_throws_typeerror_toprimitive() => ExecutionTestFromFile("toFixed-tonumber-throws-typeerror-toprimitive");
}
