using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.prototype;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.Promise.prototype") { }

    [Fact(DisplayName = "S25.4.4.2_A1.1_T1")]
    public Task S25_4_4_2_A1_1_T1() => ExecutionTestFromFile("S25.4.4.2_A1.1_T1");

    [Fact(DisplayName = "S25.4.5_A3.1_T1")]
    public Task S25_4_5_A3_1_T1() => ExecutionTestFromFile("S25.4.5_A3.1_T1");

    [Fact(DisplayName = "Symbol.toStringTag")]
    public Task Symbol_toStringTag() => ExecutionTestFromFile("Symbol.toStringTag");

    [Fact(DisplayName = "no-promise-state")]
    public Task no_promise_state() => ExecutionTestFromFile("no-promise-state");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto")]
    public Task proto() => ExecutionTestFromFile("proto");
}
