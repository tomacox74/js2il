using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.language.expressions.async_function;

public sealed class NativePortBatch_Test_02bf3372_c399_58f9_994f_d0066e480749 : DiskExecutionTestsBase
{
    public NativePortBatch_Test_02bf3372_c399_58f9_994f_d0066e480749() : base("Jroc.Test262.Tests.language.expressions.async_function") { }

    [Fact(DisplayName = "named-dflt-params-abrupt")]
    public Task named_dflt_params_abrupt() => ExecutionTestFromFile("named-dflt-params-abrupt");

    [Fact(DisplayName = "named-dflt-params-arg-val-not-undefined")]
    public Task named_dflt_params_arg_val_not_undefined() => ExecutionTestFromFile("named-dflt-params-arg-val-not-undefined");

    [Fact(DisplayName = "named-dflt-params-arg-val-undefined")]
    public Task named_dflt_params_arg_val_undefined() => ExecutionTestFromFile("named-dflt-params-arg-val-undefined");

    [Fact(DisplayName = "named-dflt-params-ref-later")]
    public Task named_dflt_params_ref_later() => ExecutionTestFromFile("named-dflt-params-ref-later");

    [Fact(DisplayName = "named-dflt-params-ref-prior")]
    public Task named_dflt_params_ref_prior() => ExecutionTestFromFile("named-dflt-params-ref-prior");

    [Fact(DisplayName = "named-dflt-params-ref-self")]
    public Task named_dflt_params_ref_self() => ExecutionTestFromFile("named-dflt-params-ref-self");

    [Fact(DisplayName = "named-dflt-params-trailing-comma")]
    public Task named_dflt_params_trailing_comma() => ExecutionTestFromFile("named-dflt-params-trailing-comma");

    [Fact(DisplayName = "named-params-trailing-comma-multiple")]
    public Task named_params_trailing_comma_multiple() => ExecutionTestFromFile("named-params-trailing-comma-multiple");

}
