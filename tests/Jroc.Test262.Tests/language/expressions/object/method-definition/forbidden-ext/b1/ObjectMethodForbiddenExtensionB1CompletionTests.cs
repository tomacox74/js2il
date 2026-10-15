namespace Jroc.Test262.Tests.language.expressions.object_.method_definition.forbidden_ext.b1;

public sealed class ObjectMethodForbiddenExtensionB1CompletionTests : DiskExecutionTestsBase
{
    public ObjectMethodForbiddenExtensionB1CompletionTests() : base("language.expressions.object_.method_definition.forbidden_ext.b1") { }

    [Fact(DisplayName = "async-gen-meth-forbidden-ext-direct-access-prop-arguments.js")]
    public Task test_async_gen_meth_forbidden_ext_direct_access_prop_arguments() => ExecutionTest("async-gen-meth-forbidden-ext-direct-access-prop-arguments");

    [Fact(DisplayName = "async-gen-meth-forbidden-ext-direct-access-prop-caller.js")]
    public Task test_async_gen_meth_forbidden_ext_direct_access_prop_caller() => ExecutionTest("async-gen-meth-forbidden-ext-direct-access-prop-caller");

    [Fact(DisplayName = "async-meth-forbidden-ext-direct-access-prop-arguments.js")]
    public Task test_async_meth_forbidden_ext_direct_access_prop_arguments() => ExecutionTest("async-meth-forbidden-ext-direct-access-prop-arguments");

    [Fact(DisplayName = "async-meth-forbidden-ext-direct-access-prop-caller.js")]
    public Task test_async_meth_forbidden_ext_direct_access_prop_caller() => ExecutionTest("async-meth-forbidden-ext-direct-access-prop-caller");

    [Fact(DisplayName = "meth-forbidden-ext-direct-access-prop-arguments.js")]
    public Task test_meth_forbidden_ext_direct_access_prop_arguments() => ExecutionTest("meth-forbidden-ext-direct-access-prop-arguments");

    [Fact(DisplayName = "meth-forbidden-ext-direct-access-prop-caller.js")]
    public Task test_meth_forbidden_ext_direct_access_prop_caller() => ExecutionTest("meth-forbidden-ext-direct-access-prop-caller");
}
