using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.match;

public partial class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.match") { }

    [Fact(DisplayName = "cstm-matcher-on-bigint-primitive.js")]
    public Task cstm_matcher_on_bigint_primitive()
        => ExecutionTestFromFile("cstm-matcher-on-bigint-primitive");

    [Fact(DisplayName = "cstm-matcher-on-boolean-primitive.js")]
    public Task cstm_matcher_on_boolean_primitive()
        => ExecutionTestFromFile("cstm-matcher-on-boolean-primitive");

    [Fact(DisplayName = "cstm-matcher-on-number-primitive.js")]
    public Task cstm_matcher_on_number_primitive()
        => ExecutionTestFromFile("cstm-matcher-on-number-primitive");

    [Fact(DisplayName = "cstm-matcher-on-string-primitive.js")]
    public Task cstm_matcher_on_string_primitive()
        => ExecutionTestFromFile("cstm-matcher-on-string-primitive");

    [Fact(DisplayName = "S15.5.4.10_A1_T4")]
    public Task S15_5_4_10_A1_T4()
        => ExecutionTestFromFile("S15.5.4.10_A1_T4");

    [Fact(DisplayName = "S15.5.4.10_A1_T6")]
    public Task S15_5_4_10_A1_T6()
        => ExecutionTestFromFile("S15.5.4.10_A1_T6");

    [Fact(DisplayName = "S15.5.4.10_A1_T7")]
    public Task S15_5_4_10_A1_T7()
        => ExecutionTestFromFile("S15.5.4.10_A1_T7");

    [Fact(DisplayName = "S15.5.4.10_A1_T8")]
    public Task S15_5_4_10_A1_T8()
        => ExecutionTestFromFile("S15.5.4.10_A1_T8");

    [Fact(DisplayName = "S15.5.4.10_A1_T9")]
    public Task S15_5_4_10_A1_T9()
        => ExecutionTestFromFile("S15.5.4.10_A1_T9");

    [Fact(DisplayName = "invoke-builtin-match")]
    public Task invoke_builtin_match()
        => ExecutionTestFromFile("invoke-builtin-match");
}
