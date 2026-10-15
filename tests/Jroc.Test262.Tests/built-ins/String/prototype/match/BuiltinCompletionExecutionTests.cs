using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.match;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.String.prototype.match") { }

    [Fact(DisplayName = "S15.5.4.10_A1_T3.js", Skip = "eval is not supported.")]
    public Task test_S15_5_4_10_A1_T3() => ExecutionTest("S15.5.4.10_A1_T3");

    [Fact(DisplayName = "duplicate-named-groups-properties.js")]
    public Task test_duplicate_named_groups_properties() => ExecutionTest("duplicate-named-groups-properties");

    [Fact(DisplayName = "duplicate-named-indices-groups-properties.js")]
    public Task test_duplicate_named_indices_groups_properties() => ExecutionTest("duplicate-named-indices-groups-properties");

    [Fact(DisplayName = "regexp-prototype-match-v-u-flag.js")]
    public Task test_regexp_prototype_match_v_u_flag() => ExecutionTest("regexp-prototype-match-v-u-flag");
}
