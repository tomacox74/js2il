using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.@object;

public class Test262BatchPort20260922Round7ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260922Round7ExecutionTests() : base("language.expressions.object") { }

    [Fact(DisplayName = "scope-meth-paramsbody-var-close")]
    public Task scope_meth_paramsbody_var_close()
        => ExecutionTest("scope-meth-paramsbody-var-close");

    [Fact(DisplayName = "scope-meth-paramsbody-var-open")]
    public Task scope_meth_paramsbody_var_open()
        => ExecutionTest("scope-meth-paramsbody-var-open");

    [Fact(DisplayName = "scope-setter-paramsbody-var-close")]
    public Task scope_setter_paramsbody_var_close()
        => ExecutionTest("scope-setter-paramsbody-var-close");

    [Fact(DisplayName = "scope-setter-paramsbody-var-open")]
    public Task scope_setter_paramsbody_var_open()
        => ExecutionTest("scope-setter-paramsbody-var-open");

    [Fact(DisplayName = "setter-length-dflt")]
    public Task setter_length_dflt()
        => ExecutionTest("setter-length-dflt");

    [Fact(DisplayName = "setter-prop-desc")]
    public Task setter_prop_desc()
        => ExecutionTest("setter-prop-desc");

    [Fact(DisplayName = "yield-non-strict-access")]
    public Task yield_non_strict_access()
        => ExecutionTest("yield-non-strict-access");

    [Fact(DisplayName = "yield-non-strict-syntax")]
    public Task yield_non_strict_syntax()
        => ExecutionTest("yield-non-strict-syntax");

}
