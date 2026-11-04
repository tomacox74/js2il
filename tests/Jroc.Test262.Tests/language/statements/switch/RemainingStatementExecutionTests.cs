namespace Jroc.Test262.Tests.language.statements.switch_;

public sealed class RemainingStatementExecutionTests : DiskExecutionTestsBase
{
    public RemainingStatementExecutionTests() : base("Jroc.Test262.Tests.language.statements.switch_") { }

    [Fact(DisplayName = "cptn-a-abrupt-empty.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_a_abrupt_empty()
        => ExecutionTest("cptn-a-abrupt-empty");

    [Fact(DisplayName = "cptn-a-fall-thru-abrupt-empty.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_a_fall_thru_abrupt_empty()
        => ExecutionTest("cptn-a-fall-thru-abrupt-empty");

    [Fact(DisplayName = "cptn-a-fall-thru-nrml.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_a_fall_thru_nrml()
        => ExecutionTest("cptn-a-fall-thru-nrml");

    [Fact(DisplayName = "cptn-abrupt-empty.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_abrupt_empty()
        => ExecutionTest("cptn-abrupt-empty");

    [Fact(DisplayName = "cptn-b-abrupt-empty.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_b_abrupt_empty()
        => ExecutionTest("cptn-b-abrupt-empty");

    [Fact(DisplayName = "cptn-b-fall-thru-abrupt-empty.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_b_fall_thru_abrupt_empty()
        => ExecutionTest("cptn-b-fall-thru-abrupt-empty");

    [Fact(DisplayName = "cptn-b-fall-thru-nrml.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_b_fall_thru_nrml()
        => ExecutionTest("cptn-b-fall-thru-nrml");

    [Fact(DisplayName = "cptn-b-final.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_b_final()
        => ExecutionTest("cptn-b-final");

    [Fact(DisplayName = "cptn-dflt-abrupt-empty.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_dflt_abrupt_empty()
        => ExecutionTest("cptn-dflt-abrupt-empty");

    [Fact(DisplayName = "cptn-dflt-b-abrupt-empty.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_dflt_b_abrupt_empty()
        => ExecutionTest("cptn-dflt-b-abrupt-empty");

    [Fact(DisplayName = "cptn-dflt-b-fall-thru-abrupt-empty.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_dflt_b_fall_thru_abrupt_empty()
        => ExecutionTest("cptn-dflt-b-fall-thru-abrupt-empty");

    [Fact(DisplayName = "cptn-dflt-b-fall-thru-nrml.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_dflt_b_fall_thru_nrml()
        => ExecutionTest("cptn-dflt-b-fall-thru-nrml");

    [Fact(DisplayName = "cptn-dflt-b-final.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_dflt_b_final()
        => ExecutionTest("cptn-dflt-b-final");

    [Fact(DisplayName = "cptn-dflt-fall-thru-abrupt-empty.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_dflt_fall_thru_abrupt_empty()
        => ExecutionTest("cptn-dflt-fall-thru-abrupt-empty");

    [Fact(DisplayName = "cptn-dflt-fall-thru-nrml.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_dflt_fall_thru_nrml()
        => ExecutionTest("cptn-dflt-fall-thru-nrml");

    [Fact(DisplayName = "cptn-dflt-final.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_dflt_final()
        => ExecutionTest("cptn-dflt-final");

    [Fact(DisplayName = "cptn-no-dflt-match-abrupt-empty.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_no_dflt_match_abrupt_empty()
        => ExecutionTest("cptn-no-dflt-match-abrupt-empty");

    [Fact(DisplayName = "cptn-no-dflt-match-fall-thru-abrupt-empty.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_no_dflt_match_fall_thru_abrupt_empty()
        => ExecutionTest("cptn-no-dflt-match-fall-thru-abrupt-empty");

    [Fact(DisplayName = "cptn-no-dflt-match-fall-thru-nrml.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_no_dflt_match_fall_thru_nrml()
        => ExecutionTest("cptn-no-dflt-match-fall-thru-nrml");

    [Fact(DisplayName = "cptn-no-dflt-match-final.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_no_dflt_match_final()
        => ExecutionTest("cptn-no-dflt-match-final");

    [Fact(DisplayName = "cptn-no-dflt-no-match.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_cptn_no_dflt_no_match()
        => ExecutionTest("cptn-no-dflt-no-match");

    [Fact(DisplayName = "scope-lex-async-generator.js")]
    public Task test_scope_lex_async_generator()
        => ExecutionTest("scope-lex-async-generator", allowUnhandledException: true);

    [Fact(DisplayName = "scope-var-none-case.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_scope_var_none_case()
        => ExecutionTest("scope-var-none-case");

    [Fact(DisplayName = "scope-var-none-dflt.js", Skip = "Supported gap: runtime eval is not supported.")]
    public Task test_scope_var_none_dflt()
        => ExecutionTest("scope-var-none-dflt");

}
