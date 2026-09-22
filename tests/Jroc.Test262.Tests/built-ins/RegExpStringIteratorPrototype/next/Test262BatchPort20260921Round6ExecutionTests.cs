using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExpStringIteratorPrototype.next;

public class Test262BatchPort20260921Round6ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260921Round6ExecutionTests() : base("built_ins.RegExpStringIteratorPrototype.next") { }

    [Fact(DisplayName = "custom-regexpexec-call-throws")]
    public Task custom_regexpexec_call_throws()
        => ExecutionTestFromFile("custom-regexpexec-call-throws");

    [Fact(DisplayName = "custom-regexpexec-match-get-0-throws")]
    public Task custom_regexpexec_match_get_0_throws()
        => ExecutionTestFromFile("custom-regexpexec-match-get-0-throws");

    [Fact(DisplayName = "custom-regexpexec-match-get-0-tostring-throws")]
    public Task custom_regexpexec_match_get_0_tostring_throws()
        => ExecutionTestFromFile("custom-regexpexec-match-get-0-tostring-throws");

    [Fact(DisplayName = "custom-regexpexec-match-get-0-tostring")]
    public Task custom_regexpexec_match_get_0_tostring()
        => ExecutionTestFromFile("custom-regexpexec-match-get-0-tostring");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "next-missing-internal-slots")]
    public Task next_missing_internal_slots()
        => ExecutionTestFromFile("next-missing-internal-slots");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "regexp-tolength-lastindex-throws")]
    public Task regexp_tolength_lastindex_throws()
        => ExecutionTestFromFile("regexp-tolength-lastindex-throws");

    [Fact(DisplayName = "this-is-not-object-throws")]
    public Task this_is_not_object_throws()
        => ExecutionTestFromFile("this-is-not-object-throws");

}
