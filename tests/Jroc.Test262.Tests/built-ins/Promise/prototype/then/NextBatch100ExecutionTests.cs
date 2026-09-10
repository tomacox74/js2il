using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.prototype.then;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.Promise.prototype.then") { }

    [Fact(DisplayName = "S25.4.5.3_A1.1_T1")]
    public Task S25_4_5_3_A1_1_T1() => ExecutionTestFromFile("S25.4.5.3_A1.1_T1");

    [Fact(DisplayName = "S25.4.5.3_A1.1_T2")]
    public Task S25_4_5_3_A1_1_T2() => ExecutionTestFromFile("S25.4.5.3_A1.1_T2");

    [Fact(DisplayName = "S25.4.5.3_A2.1_T1")]
    public Task S25_4_5_3_A2_1_T1() => ExecutionTestFromFile("S25.4.5.3_A2.1_T1");

    [Fact(DisplayName = "S25.4.5.3_A2.1_T2")]
    public Task S25_4_5_3_A2_1_T2() => ExecutionTestFromFile("S25.4.5.3_A2.1_T2");

    [Fact(DisplayName = "context-check-on-entry")]
    public Task context_check_on_entry() => ExecutionTestFromFile("context-check-on-entry");

    [Fact(DisplayName = "ctor-undef")]
    public Task ctor_undef() => ExecutionTestFromFile("ctor-undef");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
}
