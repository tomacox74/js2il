using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.function;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.function") { }

    [Fact(DisplayName = "scope-param-elem-var-close", Skip = "Tracked by #1093: current runtime behavior does not yet pass this upstream expression test.")]
    public Task scope_param_elem_var_close()
        => ExecutionTest("scope-param-elem-var-close");

    [Fact(DisplayName = "scope-param-elem-var-open", Skip = "Tracked by #1093: current runtime behavior does not yet pass this upstream expression test.")]
    public Task scope_param_elem_var_open()
        => ExecutionTest("scope-param-elem-var-open");

    [Fact(DisplayName = "scope-param-rest-elem-var-close", Skip = "Tracked by #1093: current runtime behavior does not yet pass this upstream expression test.")]
    public Task scope_param_rest_elem_var_close()
        => ExecutionTest("scope-param-rest-elem-var-close");

    [Fact(DisplayName = "scope-param-rest-elem-var-open", Skip = "Tracked by #1093: current runtime behavior does not yet pass this upstream expression test.")]
    public Task scope_param_rest_elem_var_open()
        => ExecutionTest("scope-param-rest-elem-var-open");

    [Fact(DisplayName = "scope-paramsbody-var-close")]
    public Task scope_paramsbody_var_close()
        => ExecutionTest("scope-paramsbody-var-close");

    [Fact(DisplayName = "scope-paramsbody-var-open")]
    public Task scope_paramsbody_var_open()
        => ExecutionTest("scope-paramsbody-var-open");
}
