using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Math;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Math") { }

    [Fact(DisplayName = "ceil/not-a-constructor")]
    public Task ceil_not_a_constructor()
        => ExecutionTestFromFile("ceil/not-a-constructor");

    [Fact(DisplayName = "S15.8.2.6_A6")]
    public Task ceil_S15_8_2_6_A6()
        => ExecutionTestFromFile("ceil/S15.8.2.6_A6");

    [Fact(DisplayName = "S15.8.2.12_A1")]
    public Task min_S15_8_2_12_A1()
        => ExecutionTestFromFile("min/S15.8.2.12_A1");

    [Fact(DisplayName = "15.8.2.12-1")]
    public Task min_15_8_2_12_1()
        => ExecutionTestFromFile("min/15.8.2.12-1");

    [Fact(DisplayName = "round/not-a-constructor")]
    public Task round_not_a_constructor()
        => ExecutionTestFromFile("round/not-a-constructor");

    [Fact(DisplayName = "S15.8.2.15_A4")]
    public Task round_S15_8_2_15_A4()
        => ExecutionTestFromFile("round/S15.8.2.15_A4");

    [Fact(DisplayName = "sign-specialVals")]
    public Task sign_sign_specialVals()
        => ExecutionTestFromFile("sign/sign-specialVals");

    [Fact(DisplayName = "sign/not-a-constructor")]
    public Task sign_not_a_constructor()
        => ExecutionTestFromFile("sign/not-a-constructor");
    [Fact(DisplayName = "E/value")]
    public Task E_value()
        => ExecutionTestFromFile("E/value");

    [Fact(DisplayName = "E/prop-desc")]
    public Task E_prop_desc()
        => ExecutionTestFromFile("E/prop-desc");

    [Fact(DisplayName = "LN10/value")]
    public Task LN10_value()
        => ExecutionTestFromFile("LN10/value");

    [Fact(DisplayName = "LN10/prop-desc")]
    public Task LN10_prop_desc()
        => ExecutionTestFromFile("LN10/prop-desc");

    [Fact(DisplayName = "LN2/value")]
    public Task LN2_value()
        => ExecutionTestFromFile("LN2/value");

    [Fact(DisplayName = "LN2/prop-desc")]
    public Task LN2_prop_desc()
        => ExecutionTestFromFile("LN2/prop-desc");

    [Fact(DisplayName = "LOG10E/value")]
    public Task LOG10E_value()
        => ExecutionTestFromFile("LOG10E/value");

    [Fact(DisplayName = "LOG2E/value")]
    public Task LOG2E_value()
        => ExecutionTestFromFile("LOG2E/value");

    [Fact(DisplayName = "PI/value")]
    public Task PI_value()
        => ExecutionTestFromFile("PI/value");

    [Fact(DisplayName = "SQRT2/value")]
    public Task SQRT2_value()
        => ExecutionTestFromFile("SQRT2/value");

    [Fact(DisplayName = "sqrt/S15.8.2.17_A1")]
    public Task sqrt_S15_8_2_17_A1()
        => ExecutionTestFromFile("sqrt/S15.8.2.17_A1");

    [Fact(DisplayName = "sqrt/S15.8.2.17_A2")]
    public Task sqrt_S15_8_2_17_A2()
        => ExecutionTestFromFile("sqrt/S15.8.2.17_A2");

    [Fact(DisplayName = "sqrt/S15.8.2.17_A5")]
    public Task sqrt_S15_8_2_17_A5()
        => ExecutionTestFromFile("sqrt/S15.8.2.17_A5");

    [Fact(DisplayName = "SQRT1_2/value")]
    public Task SQRT1_2_value()
        => ExecutionTestFromFile("SQRT1_2/value");

    [Fact(DisplayName = "SQRT1_2/prop-desc")]
    public Task SQRT1_2_prop_desc()
        => ExecutionTestFromFile("SQRT1_2/prop-desc");

    [Fact(DisplayName = "acosh/arg-is-one")]
    public Task acosh_arg_is_one()
        => ExecutionTestFromFile("acosh/arg-is-one");

    [Fact(DisplayName = "acosh/not-a-constructor")]
    public Task acosh_not_a_constructor()
        => ExecutionTestFromFile("acosh/not-a-constructor");

    [Fact(DisplayName = "asinh/asinh-specialVals")]
    public Task asinh_asinh_specialVals()
        => ExecutionTestFromFile("asinh/asinh-specialVals");

    [Fact(DisplayName = "asinh/not-a-constructor")]
    public Task asinh_not_a_constructor()
        => ExecutionTestFromFile("asinh/not-a-constructor");

    [Fact(DisplayName = "atanh/atanh-specialVals")]
    public Task atanh_atanh_specialVals()
        => ExecutionTestFromFile("atanh/atanh-specialVals");

    [Fact(DisplayName = "atanh/not-a-constructor")]
    public Task atanh_not_a_constructor()
        => ExecutionTestFromFile("atanh/not-a-constructor");

    [Fact(DisplayName = "atan2/S15.8.2.5_A1")]
    public Task atan2_S15_8_2_5_A1()
        => ExecutionTestFromFile("atan2/S15.8.2.5_A1");

    [Fact(DisplayName = "atan2/not-a-constructor")]
    public Task atan2_not_a_constructor()
        => ExecutionTestFromFile("atan2/not-a-constructor");

    [Fact(DisplayName = "cbrt/cbrt-specialValues")]
    public Task cbrt_cbrt_specialValues()
        => ExecutionTestFromFile("cbrt/cbrt-specialValues");

    [Fact(DisplayName = "cbrt/not-a-constructor")]
    public Task cbrt_not_a_constructor()
        => ExecutionTestFromFile("cbrt/not-a-constructor");

    [Fact(DisplayName = "clz32/int32bit")]
    public Task clz32_int32bit()
        => ExecutionTestFromFile("clz32/int32bit");

    [Fact(DisplayName = "clz32/not-a-constructor")]
    public Task clz32_not_a_constructor()
        => ExecutionTestFromFile("clz32/not-a-constructor");

    [Fact(DisplayName = "cosh/cosh-specialVals")]
    public Task cosh_cosh_specialVals()
        => ExecutionTestFromFile("cosh/cosh-specialVals");

    [Fact(DisplayName = "cosh/not-a-constructor")]
    public Task cosh_not_a_constructor()
        => ExecutionTestFromFile("cosh/not-a-constructor");

    [Fact(DisplayName = "expm1/expm1-specialVals")]
    public Task expm1_expm1_specialVals()
        => ExecutionTestFromFile("expm1/expm1-specialVals");

    [Fact(DisplayName = "expm1/not-a-constructor")]
    public Task expm1_not_a_constructor()
        => ExecutionTestFromFile("expm1/not-a-constructor");

    [Fact(DisplayName = "fround/Math.fround_Zero")]
    public Task fround_Math_fround_Zero()
        => ExecutionTestFromFile("fround/Math.fround_Zero");

    [Fact(DisplayName = "fround/not-a-constructor")]
    public Task fround_not_a_constructor()
        => ExecutionTestFromFile("fround/not-a-constructor");

    [Fact(DisplayName = "f16round/length")]
    public Task f16round_length()
        => ExecutionTestFromFile("f16round/length");

    [Fact(DisplayName = "f16round/not-a-constructor")]
    public Task f16round_not_a_constructor()
        => ExecutionTestFromFile("f16round/not-a-constructor");

    [Fact(DisplayName = "hypot/Math.hypot_Success_2")]
    public Task hypot_Math_hypot_Success_2()
        => ExecutionTestFromFile("hypot/Math.hypot_Success_2");

    [Fact(DisplayName = "hypot/not-a-constructor")]
    public Task hypot_not_a_constructor()
        => ExecutionTestFromFile("hypot/not-a-constructor");

    [Fact(DisplayName = "imul/results")]
    public Task imul_results()
        => ExecutionTestFromFile("imul/results");

    [Fact(DisplayName = "imul/not-a-constructor")]
    public Task imul_not_a_constructor()
        => ExecutionTestFromFile("imul/not-a-constructor");

    [Fact(DisplayName = "log/S15.8.2.10_A1")]
    public Task log_S15_8_2_10_A1()
        => ExecutionTestFromFile("log/S15.8.2.10_A1");

    [Fact(DisplayName = "log/not-a-constructor")]
    public Task log_not_a_constructor()
        => ExecutionTestFromFile("log/not-a-constructor");

    [Fact(DisplayName = "log1p/specific-results")]
    public Task log1p_specific_results()
        => ExecutionTestFromFile("log1p/specific-results");

    [Fact(DisplayName = "log1p/not-a-constructor")]
    public Task log1p_not_a_constructor()
        => ExecutionTestFromFile("log1p/not-a-constructor");

    [Fact(DisplayName = "log10/Log10-specialVals")]
    public Task log10_Log10_specialVals()
        => ExecutionTestFromFile("log10/Log10-specialVals");

    [Fact(DisplayName = "log10/not-a-constructor")]
    public Task log10_not_a_constructor()
        => ExecutionTestFromFile("log10/not-a-constructor");

    [Fact(DisplayName = "log2/log2-basicTests")]
    public Task log2_log2_basicTests()
        => ExecutionTestFromFile("log2/log2-basicTests");

    [Fact(DisplayName = "log2/not-a-constructor")]
    public Task log2_not_a_constructor()
        => ExecutionTestFromFile("log2/not-a-constructor");

    [Fact(DisplayName = "pow/applying-the-exp-operator_A1")]
    public Task pow_applying_the_exp_operator_A1()
        => ExecutionTestFromFile("pow/applying-the-exp-operator_A1");

    [Fact(DisplayName = "pow/not-a-constructor")]
    public Task pow_not_a_constructor()
        => ExecutionTestFromFile("pow/not-a-constructor");

    [Fact(DisplayName = "random/S15.8.2.14_A1")]
    public Task random_S15_8_2_14_A1()
        => ExecutionTestFromFile("random/S15.8.2.14_A1");

    [Fact(DisplayName = "random/not-a-constructor")]
    public Task random_not_a_constructor()
        => ExecutionTestFromFile("random/not-a-constructor");

    [Fact(DisplayName = "sin/zero")]
    public Task sin_zero()
        => ExecutionTestFromFile("sin/zero");

    [Fact(DisplayName = "sin/not-a-constructor")]
    public Task sin_not_a_constructor()
        => ExecutionTestFromFile("sin/not-a-constructor");

    [Fact(DisplayName = "sinh/sinh-specialVals")]
    public Task sinh_sinh_specialVals()
        => ExecutionTestFromFile("sinh/sinh-specialVals");

    [Fact(DisplayName = "sinh/not-a-constructor")]
    public Task sinh_not_a_constructor()
        => ExecutionTestFromFile("sinh/not-a-constructor");

    [Fact(DisplayName = "sumPrecise/length")]
    public Task sumPrecise_length()
        => ExecutionTestFromFile("sumPrecise/length");

    [Fact(DisplayName = "sumPrecise/not-a-constructor")]
    public Task sumPrecise_not_a_constructor()
        => ExecutionTestFromFile("sumPrecise/not-a-constructor");

    [Fact(DisplayName = "tan/S15.8.2.18_A1")]
    public Task tan_S15_8_2_18_A1()
        => ExecutionTestFromFile("tan/S15.8.2.18_A1");

    [Fact(DisplayName = "tan/not-a-constructor")]
    public Task tan_not_a_constructor()
        => ExecutionTestFromFile("tan/not-a-constructor");

    [Fact(DisplayName = "tanh/tanh-specialVals")]
    public Task tanh_tanh_specialVals()
        => ExecutionTestFromFile("tanh/tanh-specialVals");

    [Fact(DisplayName = "tanh/not-a-constructor")]
    public Task tanh_not_a_constructor()
        => ExecutionTestFromFile("tanh/not-a-constructor");

    [Fact(DisplayName = "trunc/trunc-specialVals")]
    public Task trunc_trunc_specialVals()
        => ExecutionTestFromFile("trunc/trunc-specialVals");

    [Fact(DisplayName = "trunc/not-a-constructor")]
    public Task trunc_not_a_constructor()
        => ExecutionTestFromFile("trunc/not-a-constructor");

    [Fact(DisplayName = "Symbol.toStringTag")]
    public Task Symbol_toStringTag()
        => ExecutionTestFromFile("Symbol.toStringTag");

}
