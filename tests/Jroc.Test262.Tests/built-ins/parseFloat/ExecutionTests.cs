using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.parseFloat;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.parseFloat") { }

    [Fact(DisplayName = "15.1.2.3-2-1")]
    public Task _15_1_2_3_2_1()
        => ExecutionTestFromFile("15.1.2.3-2-1");

    [Fact(DisplayName = "S15.1.2.3_A1_T1")]
    public Task S15_1_2_3_A1_T1()
        => ExecutionTestFromFile("S15.1.2.3_A1_T1");

    [Fact(DisplayName = "S15.1.2.3_A1_T3")]
    public Task S15_1_2_3_A1_T3()
        => ExecutionTestFromFile("S15.1.2.3_A1_T3");

    [Fact(DisplayName = "S15.1.2.3_A1_T4")]
    public Task S15_1_2_3_A1_T4()
        => ExecutionTestFromFile("S15.1.2.3_A1_T4");

    [Fact(DisplayName = "S15.1.2.3_A2_T1")]
    public Task S15_1_2_3_A2_T1()
        => ExecutionTestFromFile("S15.1.2.3_A2_T1");

    [Fact(DisplayName = "S15.1.2.3_A2_T2")]
    public Task S15_1_2_3_A2_T2()
        => ExecutionTestFromFile("S15.1.2.3_A2_T2");

    [Fact(DisplayName = "S15.1.2.3_A2_T3")]
    public Task S15_1_2_3_A2_T3()
        => ExecutionTestFromFile("S15.1.2.3_A2_T3");

    [Fact(DisplayName = "S15.1.2.3_A4_T1")]
    public Task S15_1_2_3_A4_T1()
        => ExecutionTestFromFile("S15.1.2.3_A4_T1");

    [Fact(DisplayName = "S15.1.2.3_A4_T2")]
    public Task S15_1_2_3_A4_T2()
        => ExecutionTestFromFile("S15.1.2.3_A4_T2");

    [Fact(DisplayName = "S15.1.2.3_A5_T1")]
    public Task S15_1_2_3_A5_T1()
        => ExecutionTestFromFile("S15.1.2.3_A5_T1");

    [Fact(DisplayName = "S15.1.2.3_A5_T2")]
    public Task S15_1_2_3_A5_T2()
        => ExecutionTestFromFile("S15.1.2.3_A5_T2");

    [Fact(DisplayName = "S15.1.2.3_A6")]
    public Task S15_1_2_3_A6()
        => ExecutionTestFromFile("S15.1.2.3_A6");

    [Fact(DisplayName = "S15.1.2.3_A2_T10_U180E.js")]
    public Task Port400_S15_1_2_3_A2_T10_U180E()
        => ExecutionTestFromFile("S15.1.2.3_A2_T10_U180E");

    [Fact(DisplayName = "S15.1.2.3_A4_T5.js")]
    public Task Port400_S15_1_2_3_A4_T5()
        => ExecutionTestFromFile("S15.1.2.3_A4_T5");

    [Fact(DisplayName = "S15.1.2.3_A4_T6.js")]
    public Task Port400_S15_1_2_3_A4_T6()
        => ExecutionTestFromFile("S15.1.2.3_A4_T6");

    [Fact(DisplayName = "S15.1.2.3_A4_T7.js")]
    public Task Port400_S15_1_2_3_A4_T7()
        => ExecutionTestFromFile("S15.1.2.3_A4_T7");

    [Fact(DisplayName = "S15.1.2.3_A7.5.js")]
    public Task Port400_S15_1_2_3_A7_5()
        => ExecutionTestFromFile("S15.1.2.3_A7.5");

    [Fact(DisplayName = "S15.1.2.3_A7.6.js")]
    public Task Port400_S15_1_2_3_A7_6()
        => ExecutionTestFromFile("S15.1.2.3_A7.6");

    [Fact(DisplayName = "S15.1.2.3_A7.7.js")]
    public Task Port400_S15_1_2_3_A7_7()
        => ExecutionTestFromFile("S15.1.2.3_A7.7");

    [Fact(DisplayName = "tonumber-numeric-separator-literal-dd-dot-dd-ep-sign-minus-dd-nsl-dd.js")]
    public Task Port400_tonumber_numeric_separator_literal_dd_dot_dd_ep_sign_minus_dd_nsl_dd()
        => ExecutionTestFromFile("tonumber-numeric-separator-literal-dd-dot-dd-ep-sign-minus-dd-nsl-dd");

    [Fact(DisplayName = "tonumber-numeric-separator-literal-dd-dot-dd-ep-sign-minus-dds-nsl-dd.js")]
    public Task Port400_tonumber_numeric_separator_literal_dd_dot_dd_ep_sign_minus_dds_nsl_dd()
        => ExecutionTestFromFile("tonumber-numeric-separator-literal-dd-dot-dd-ep-sign-minus-dds-nsl-dd");

    [Fact(DisplayName = "tonumber-numeric-separator-literal-dd-dot-dd-ep-sign-plus-dd-nsl-dd.js")]
    public Task Port400_tonumber_numeric_separator_literal_dd_dot_dd_ep_sign_plus_dd_nsl_dd()
        => ExecutionTestFromFile("tonumber-numeric-separator-literal-dd-dot-dd-ep-sign-plus-dd-nsl-dd");

    [Fact(DisplayName = "tonumber-numeric-separator-literal-dd-dot-dd-ep-sign-plus-dds-nsl-dd.js")]
    public Task Port400_tonumber_numeric_separator_literal_dd_dot_dd_ep_sign_plus_dds_nsl_dd()
        => ExecutionTestFromFile("tonumber-numeric-separator-literal-dd-dot-dd-ep-sign-plus-dds-nsl-dd");

    [Fact(DisplayName = "tonumber-numeric-separator-literal-dd-nsl-dd-one-of.js")]
    public Task Port400_tonumber_numeric_separator_literal_dd_nsl_dd_one_of()
        => ExecutionTestFromFile("tonumber-numeric-separator-literal-dd-nsl-dd-one-of");

    [Fact(DisplayName = "tonumber-numeric-separator-literal-dds-dot-dd-nsl-dd-ep-dd.js")]
    public Task Port400_tonumber_numeric_separator_literal_dds_dot_dd_nsl_dd_ep_dd()
        => ExecutionTestFromFile("tonumber-numeric-separator-literal-dds-dot-dd-nsl-dd-ep-dd");

    [Fact(DisplayName = "tonumber-numeric-separator-literal-dds-nsl-dd.js")]
    public Task Port400_tonumber_numeric_separator_literal_dds_nsl_dd()
        => ExecutionTestFromFile("tonumber-numeric-separator-literal-dds-nsl-dd");

    [Fact(DisplayName = "tonumber-numeric-separator-literal-dot-dd-nsl-dd-ep.js")]
    public Task Port400_tonumber_numeric_separator_literal_dot_dd_nsl_dd_ep()
        => ExecutionTestFromFile("tonumber-numeric-separator-literal-dot-dd-nsl-dd-ep");

    [Fact(DisplayName = "tonumber-numeric-separator-literal-dot-dd-nsl-dds-ep.js")]
    public Task Port400_tonumber_numeric_separator_literal_dot_dd_nsl_dds_ep()
        => ExecutionTestFromFile("tonumber-numeric-separator-literal-dot-dd-nsl-dds-ep");

    [Fact(DisplayName = "tonumber-numeric-separator-literal-dot-dds-nsl-dd-ep.js")]
    public Task Port400_tonumber_numeric_separator_literal_dot_dds_nsl_dd_ep()
        => ExecutionTestFromFile("tonumber-numeric-separator-literal-dot-dds-nsl-dd-ep");

    [Fact(DisplayName = "tonumber-numeric-separator-literal-dot-dds-nsl-dds-ep.js")]
    public Task Port400_tonumber_numeric_separator_literal_dot_dds_nsl_dds_ep()
        => ExecutionTestFromFile("tonumber-numeric-separator-literal-dot-dds-nsl-dds-ep");

    [Fact(DisplayName = "tonumber-numeric-separator-literal-nzd-nsl-dd-one-of.js")]
    public Task Port400_tonumber_numeric_separator_literal_nzd_nsl_dd_one_of()
        => ExecutionTestFromFile("tonumber-numeric-separator-literal-nzd-nsl-dd-one-of");

    [Fact(DisplayName = "tonumber-numeric-separator-literal-nzd-nsl-dd.js")]
    public Task Port400_tonumber_numeric_separator_literal_nzd_nsl_dd()
        => ExecutionTestFromFile("tonumber-numeric-separator-literal-nzd-nsl-dd");

    [Fact(DisplayName = "tonumber-numeric-separator-literal-nzd-nsl-dds.js")]
    public Task Port400_tonumber_numeric_separator_literal_nzd_nsl_dds()
        => ExecutionTestFromFile("tonumber-numeric-separator-literal-nzd-nsl-dds");

    [Fact(DisplayName = "tonumber-numeric-separator-literal-sign-plus-dds-nsl-dd.js")]
    public Task Port400_tonumber_numeric_separator_literal_sign_plus_dds_nsl_dd()
        => ExecutionTestFromFile("tonumber-numeric-separator-literal-sign-plus-dds-nsl-dd");
}
