using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.class_;

public class PortExpressionsBatchExecutionTests : DiskExecutionTestsBase
{
    public PortExpressionsBatchExecutionTests() : base("language.expressions.class_") { }

    [Fact(DisplayName = "heritage-arrow-function", Skip = "Tracked by #1093: current runtime behavior does not yet pass this upstream expression test.")]
    public Task heritage_arrow_function()
        => ExecutionTest("heritage-arrow-function");

    [Fact(DisplayName = "poisoned-underscore-proto", Skip = "Tracked by #1093: current runtime behavior does not yet pass this upstream expression test.")]
    public Task poisoned_underscore_proto()
        => ExecutionTest("poisoned-underscore-proto");

    [Fact(DisplayName = "restricted-properties", Skip = "Tracked by #1093: current runtime behavior does not yet pass this upstream expression test.")]
    public Task restricted_properties()
        => ExecutionTest("restricted-properties");

    [Fact(DisplayName = "scope-meth-paramsbody-var-close")]
    public Task scope_meth_paramsbody_var_close()
        => ExecutionTest("scope-meth-paramsbody-var-close");

    [Fact(DisplayName = "scope-meth-paramsbody-var-open")]
    public Task scope_meth_paramsbody_var_open()
        => ExecutionTest("scope-meth-paramsbody-var-open");

    [Fact(DisplayName = "scope-name-lex-close")]
    public Task scope_name_lex_close()
        => ExecutionTest("scope-name-lex-close");

    [Fact(DisplayName = "scope-name-lex-open-heritage")]
    public Task scope_name_lex_open_heritage()
        => ExecutionTest("scope-name-lex-open-heritage");

    [Fact(DisplayName = "scope-name-lex-open-no-heritage")]
    public Task scope_name_lex_open_no_heritage()
        => ExecutionTest("scope-name-lex-open-no-heritage");

    [Fact(DisplayName = "scope-setter-paramsbody-var-close")]
    public Task scope_setter_paramsbody_var_close()
        => ExecutionTest("scope-setter-paramsbody-var-close");

    [Fact(DisplayName = "scope-setter-paramsbody-var-open")]
    public Task scope_setter_paramsbody_var_open()
        => ExecutionTest("scope-setter-paramsbody-var-open");

    [Fact(DisplayName = "scope-static-meth-paramsbody-var-close")]
    public Task scope_static_meth_paramsbody_var_close()
        => ExecutionTest("scope-static-meth-paramsbody-var-close");

    [Fact(DisplayName = "scope-static-meth-paramsbody-var-open")]
    public Task scope_static_meth_paramsbody_var_open()
        => ExecutionTest("scope-static-meth-paramsbody-var-open");
}
