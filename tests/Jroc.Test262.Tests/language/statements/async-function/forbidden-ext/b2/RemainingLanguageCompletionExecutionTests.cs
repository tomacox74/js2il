namespace Jroc.Test262.Tests.language.statements.async_function.forbidden_ext.b2;

public sealed class RemainingLanguageCompletionExecutionTests : DiskExecutionTestsBase
{
    public RemainingLanguageCompletionExecutionTests() : base("language.statements.async_function.forbidden_ext.b2") { }

    [Fact(DisplayName = "async-func-decl-forbidden-ext-indirect-access-own-prop-caller-get.js")]
    public Task async_func_decl_forbidden_ext_indirect_access_own_prop_caller_get()
        => ExecutionTest("async-func-decl-forbidden-ext-indirect-access-own-prop-caller-get");

    [Fact(DisplayName = "async-func-decl-forbidden-ext-indirect-access-own-prop-caller-value.js")]
    public Task async_func_decl_forbidden_ext_indirect_access_own_prop_caller_value()
        => ExecutionTest("async-func-decl-forbidden-ext-indirect-access-own-prop-caller-value");

    [Fact(DisplayName = "async-func-decl-forbidden-ext-indirect-access-prop-caller.js")]
    public Task async_func_decl_forbidden_ext_indirect_access_prop_caller()
        => ExecutionTest("async-func-decl-forbidden-ext-indirect-access-prop-caller");
}
