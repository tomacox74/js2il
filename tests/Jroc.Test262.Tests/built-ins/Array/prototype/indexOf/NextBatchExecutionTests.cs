using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.indexOf;

public class NextBatchExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.Array.prototype.indexOf") { }

    [Fact(DisplayName = "15.4.4.14-5-19")]
    public Task _15_4_4_14_5_19() => ExecutionTestFromFile("15.4.4.14-5-19");

    [Fact(DisplayName = "15.4.4.14-5-24")]
    public Task _15_4_4_14_5_24() => ExecutionTestFromFile("15.4.4.14-5-24");

    [Fact(DisplayName = "15.4.4.14-9-9")]
    public Task _15_4_4_14_9_9() => ExecutionTestFromFile("15.4.4.14-9-9");

    [Fact(DisplayName = "15.4.4.14-9-a-17")]
    public Task _15_4_4_14_9_a_17() => ExecutionTestFromFile("15.4.4.14-9-a-17");

    [Fact(DisplayName = "15.4.4.14-9-a-18")]
    public Task _15_4_4_14_9_a_18() => ExecutionTestFromFile("15.4.4.14-9-a-18");

    [Fact(DisplayName = "15.4.4.14-9-b-1")]
    public Task _15_4_4_14_9_b_1() => ExecutionTestFromFile("15.4.4.14-9-b-1");

    [Fact(DisplayName = "15.4.4.14-9-b-ii-2")]
    public Task _15_4_4_14_9_b_ii_2() => ExecutionTestFromFile("15.4.4.14-9-b-ii-2");

    [Fact(DisplayName = "calls-only-has-on-prototype-after-length-zeroed")]
    public Task calls_only_has_on_prototype_after_length_zeroed() => ExecutionTestFromFile("calls-only-has-on-prototype-after-length-zeroed");

    [Fact(DisplayName = "coerced-searchelement-fromindex-grow")]
    public Task coerced_searchelement_fromindex_grow() => ExecutionTestFromFile("coerced-searchelement-fromindex-grow");

    [Fact(DisplayName = "coerced-searchelement-fromindex-shrink")]
    public Task coerced_searchelement_fromindex_shrink() => ExecutionTestFromFile("coerced-searchelement-fromindex-shrink");

    [Fact(DisplayName = "length-near-integer-limit")]
    public Task length_near_integer_limit() => ExecutionTestFromFile("length-near-integer-limit");

}
