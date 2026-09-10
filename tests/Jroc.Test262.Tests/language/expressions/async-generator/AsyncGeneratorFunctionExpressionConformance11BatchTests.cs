using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.async_generator;

public sealed class AsyncGeneratorFunctionExpressionConformance11BatchTests : DiskExecutionTestsBase
{
    public AsyncGeneratorFunctionExpressionConformance11BatchTests() : base("language.expressions.async_generator") { }

    [Fact(DisplayName = "default-proto.js")]
    public Task test_default_proto()
        => ExecutionTest("default-proto");

    [Fact(DisplayName = "early-errors-expression-binding-identifier-eval.js")]
    public Task test_early_errors_expression_binding_identifier_eval()
        => CompilationFailureTest("early-errors-expression-binding-identifier-eval");

    [Fact(DisplayName = "named-dflt-params-abrupt.js")]
    public Task test_named_dflt_params_abrupt()
        => ExecutionTest("named-dflt-params-abrupt");

    [Fact(DisplayName = "named-dflt-params-ref-later.js")]
    public Task test_named_dflt_params_ref_later()
        => ExecutionTest("named-dflt-params-ref-later");

    [Fact(DisplayName = "named-dflt-params-ref-self.js")]
    public Task test_named_dflt_params_ref_self()
        => ExecutionTest("named-dflt-params-ref-self");

}
