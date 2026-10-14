namespace Jroc.Test262.Tests.language.statements.async_generator.forbidden_ext.b1;

public sealed class AsyncGeneratorForbiddenExtensionB1CompletionTests : DiskExecutionTestsBase
{
    public AsyncGeneratorForbiddenExtensionB1CompletionTests() : base("language.statements.async_generator.forbidden_ext.b1") { }

    [Fact(DisplayName = "async-gen-func-decl-forbidden-ext-direct-access-prop-arguments.js")]
    public Task test_async_gen_func_decl_forbidden_ext_direct_access_prop_arguments() => ExecutionTest("async-gen-func-decl-forbidden-ext-direct-access-prop-arguments");

    [Fact(DisplayName = "async-gen-func-decl-forbidden-ext-direct-access-prop-caller.js")]
    public Task test_async_gen_func_decl_forbidden_ext_direct_access_prop_caller() => ExecutionTest("async-gen-func-decl-forbidden-ext-direct-access-prop-caller");
}
