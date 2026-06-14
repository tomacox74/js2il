using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.statements.function;

public class PortAdditionalFunctionDeclarationCoverageExecutionTests : DiskExecutionTestsBase
{
    public PortAdditionalFunctionDeclarationCoverageExecutionTests() : base("language.statements.function") { }

    [Fact(DisplayName = "dflt-params-abrupt")]
    public Task dflt_params_abrupt()
        => ExecutionTest("dflt-params-abrupt");

    [Fact(DisplayName = "dflt-params-arg-val-not-undefined")]
    public Task dflt_params_arg_val_not_undefined()
        => ExecutionTest("dflt-params-arg-val-not-undefined");

    [Fact(DisplayName = "dflt-params-arg-val-undefined")]
    public Task dflt_params_arg_val_undefined()
        => ExecutionTest("dflt-params-arg-val-undefined");

    [Fact(DisplayName = "dflt-params-ref-prior")]
    public Task dflt_params_ref_prior()
        => ExecutionTest("dflt-params-ref-prior");

    [Fact(DisplayName = "dflt-params-ref-self")]
    public Task dflt_params_ref_self()
        => ExecutionTest("dflt-params-ref-self");

    [Fact(DisplayName = "length-dflt")]
    public Task length_dflt()
        => ExecutionTest("length-dflt");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTest("name");

    [Fact(DisplayName = "params-trailing-comma-multiple")]
    public Task params_trailing_comma_multiple()
        => ExecutionTest("params-trailing-comma-multiple");

    [Fact(DisplayName = "params-trailing-comma-single")]
    public Task params_trailing_comma_single()
        => ExecutionTest("params-trailing-comma-single");

    [Fact(DisplayName = "ary-name-iter-val")]
    public Task dstr_ary_name_iter_val()
        => ExecutionTest("dstr/ary-name-iter-val");

    [Fact(DisplayName = "ary-ptrn-rest-id")]
    public Task dstr_ary_ptrn_rest_id()
        => ExecutionTest("dstr/ary-ptrn-rest-id");

    [Fact(DisplayName = "S10.1.1_A1_T1")]
    public Task S10_1_1_A1_T1()
        => ExecutionTest("S10.1.1_A1_T1");
    [Fact(DisplayName = "13.2-3-s")]
    public Task _13_2_3_s()
        => ExecutionTest("13.2-3-s");
    [Fact(DisplayName = "13.2-4-s")]
    public Task _13_2_4_s()
        => ExecutionTest("13.2-4-s");
    [Fact(DisplayName = "13.2-5-s")]
    public Task _13_2_5_s()
        => ExecutionTest("13.2-5-s");
    [Fact(DisplayName = "13.2-6-s")]
    public Task _13_2_6_s()
        => ExecutionTest("13.2-6-s");
    [Fact(DisplayName = "13.2-7-s")]
    public Task _13_2_7_s()
        => ExecutionTest("13.2-7-s");
    [Fact(DisplayName = "13.2-8-s")]
    public Task _13_2_8_s()
        => ExecutionTest("13.2-8-s");
    [Fact(DisplayName = "13.2-9-s")]
    public Task _13_2_9_s()
        => ExecutionTest("13.2-9-s");
    [Fact(DisplayName = "13.2-10-s")]
    public Task _13_2_10_s()
        => ExecutionTest("13.2-10-s");
    [Fact(DisplayName = "13.2-11-s")]
    public Task _13_2_11_s()
        => ExecutionTest("13.2-11-s");
    [Fact(DisplayName = "13.2-12-s")]
    public Task _13_2_12_s()
        => ExecutionTest("13.2-12-s");
    [Fact(DisplayName = "13.2-13-s")]
    public Task _13_2_13_s()
        => ExecutionTest("13.2-13-s");
    [Fact(DisplayName = "13.2-14-s")]
    public Task _13_2_14_s()
        => ExecutionTest("13.2-14-s");
    [Fact(DisplayName = "13.2-15-s")]
    public Task _13_2_15_s()
        => ExecutionTest("13.2-15-s");
    [Fact(DisplayName = "13.2-16-s")]
    public Task _13_2_16_s()
        => ExecutionTest("13.2-16-s");
    [Fact(DisplayName = "13.2-17-s")]
    public Task _13_2_17_s()
        => ExecutionTest("13.2-17-s");
    [Fact(DisplayName = "13.2-18-s")]
    public Task _13_2_18_s()
        => ExecutionTest("13.2-18-s");
    [Fact(DisplayName = "13.2-19-s")]
    public Task _13_2_19_s()
        => ExecutionTest("13.2-19-s");
    [Fact(DisplayName = "13.2-20-s")]
    public Task _13_2_20_s()
        => ExecutionTest("13.2-20-s");
}
