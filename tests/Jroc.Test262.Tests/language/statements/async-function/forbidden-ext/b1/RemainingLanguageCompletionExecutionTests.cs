namespace Jroc.Test262.Tests.language.statements.async_function.forbidden_ext.b1;

public sealed class RemainingLanguageCompletionExecutionTests : DiskExecutionTestsBase
{
    public RemainingLanguageCompletionExecutionTests() : base("language.statements.async_function.forbidden_ext.b1") { }

    [Fact(DisplayName = "async-func-decl-forbidden-ext-direct-access-prop-arguments.js")]
    public Task async_func_decl_forbidden_ext_direct_access_prop_arguments()
        => ExecutionTest("async-func-decl-forbidden-ext-direct-access-prop-arguments");

    [Fact(DisplayName = "async-func-decl-forbidden-ext-direct-access-prop-caller.js")]
    public Task async_func_decl_forbidden_ext_direct_access_prop_caller()
        => ExecutionTest("async-func-decl-forbidden-ext-direct-access-prop-caller");
}
