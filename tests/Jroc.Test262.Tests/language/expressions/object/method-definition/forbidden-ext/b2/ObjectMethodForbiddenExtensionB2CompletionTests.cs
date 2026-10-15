namespace Jroc.Test262.Tests.language.expressions.object_.method_definition.forbidden_ext.b2;

public sealed class ObjectMethodForbiddenExtensionB2CompletionTests : DiskExecutionTestsBase
{
    public ObjectMethodForbiddenExtensionB2CompletionTests() : base("language.expressions.object_.method_definition.forbidden_ext.b2") { }

    [Fact(DisplayName = "async-gen-meth-forbidden-ext-indirect-access-own-prop-caller-get.js")]
    public Task test_async_gen_meth_forbidden_ext_indirect_access_own_prop_caller_get() => ExecutionTest("async-gen-meth-forbidden-ext-indirect-access-own-prop-caller-get");

    [Fact(DisplayName = "async-gen-meth-forbidden-ext-indirect-access-own-prop-caller-value.js")]
    public Task test_async_gen_meth_forbidden_ext_indirect_access_own_prop_caller_value() => ExecutionTest("async-gen-meth-forbidden-ext-indirect-access-own-prop-caller-value");

    [Fact(DisplayName = "async-gen-meth-forbidden-ext-indirect-access-prop-caller.js")]
    public Task test_async_gen_meth_forbidden_ext_indirect_access_prop_caller() => ExecutionTest("async-gen-meth-forbidden-ext-indirect-access-prop-caller");

    [Fact(DisplayName = "async-meth-forbidden-ext-indirect-access-own-prop-caller-get.js")]
    public Task test_async_meth_forbidden_ext_indirect_access_own_prop_caller_get() => ExecutionTest("async-meth-forbidden-ext-indirect-access-own-prop-caller-get");

    [Fact(DisplayName = "async-meth-forbidden-ext-indirect-access-own-prop-caller-value.js")]
    public Task test_async_meth_forbidden_ext_indirect_access_own_prop_caller_value() => ExecutionTest("async-meth-forbidden-ext-indirect-access-own-prop-caller-value");

    [Fact(DisplayName = "async-meth-forbidden-ext-indirect-access-prop-caller.js")]
    public Task test_async_meth_forbidden_ext_indirect_access_prop_caller() => ExecutionTest("async-meth-forbidden-ext-indirect-access-prop-caller");
}
