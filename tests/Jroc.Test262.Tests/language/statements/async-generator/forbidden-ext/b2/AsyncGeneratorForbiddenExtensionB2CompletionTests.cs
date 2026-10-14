namespace Jroc.Test262.Tests.language.statements.async_generator.forbidden_ext.b2;

public sealed class AsyncGeneratorForbiddenExtensionB2CompletionTests : DiskExecutionTestsBase
{
    public AsyncGeneratorForbiddenExtensionB2CompletionTests() : base("language.statements.async_generator.forbidden_ext.b2") { }

    [Fact(DisplayName = "async-gen-func-decl-forbidden-ext-indirect-access-own-prop-caller-get.js")]
    public Task test_async_gen_func_decl_forbidden_ext_indirect_access_own_prop_caller_get() => ExecutionTest("async-gen-func-decl-forbidden-ext-indirect-access-own-prop-caller-get");

    [Fact(DisplayName = "async-gen-func-decl-forbidden-ext-indirect-access-own-prop-caller-value.js")]
    public Task test_async_gen_func_decl_forbidden_ext_indirect_access_own_prop_caller_value() => ExecutionTest("async-gen-func-decl-forbidden-ext-indirect-access-own-prop-caller-value");

    [Fact(DisplayName = "async-gen-func-decl-forbidden-ext-indirect-access-prop-caller.js")]
    public Task test_async_gen_func_decl_forbidden_ext_indirect_access_prop_caller() => ExecutionTest("async-gen-func-decl-forbidden-ext-indirect-access-prop-caller");
}
