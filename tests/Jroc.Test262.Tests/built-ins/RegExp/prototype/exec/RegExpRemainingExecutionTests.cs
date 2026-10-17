namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.exec;

public sealed class RegExpRemainingExecutionTests : InMemoryExecutionTestsBase
{
    public RegExpRemainingExecutionTests() : base("built_ins.RegExp.prototype.exec") { }

    [Fact(DisplayName = "S15.10.6.2_A3_T2.js")]
    public Task S15_10_6_2_A3_T2()
        => ExecutionTest("S15.10.6.2_A3_T2");

    [Fact(DisplayName = "S15.10.6.2_A3_T3.js")]
    public Task S15_10_6_2_A3_T3()
        => ExecutionTest("S15.10.6.2_A3_T3");

    [Fact(DisplayName = "S15.10.6.2_A3_T4.js")]
    public Task S15_10_6_2_A3_T4()
        => ExecutionTest("S15.10.6.2_A3_T4");

    [Fact(DisplayName = "duplicate-named-groups-properties.js")]
    public Task duplicate_named_groups_properties()
        => ExecutionTest("duplicate-named-groups-properties");

    [Fact(DisplayName = "duplicate-named-indices-groups-properties.js")]
    public Task duplicate_named_indices_groups_properties()
        => ExecutionTest("duplicate-named-indices-groups-properties");

    [Fact(DisplayName = "regexp-builtin-exec-v-u-flag.js")]
    public Task regexp_builtin_exec_v_u_flag()
        => ExecutionTest("regexp-builtin-exec-v-u-flag");
}
