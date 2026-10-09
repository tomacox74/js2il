using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.language.expressions.Test_class.async_method_static.forbidden_ext.b1;

public sealed class ClassCompletionExecutionTests : DiskExecutionTestsBase
{
    public ClassCompletionExecutionTests() : base("Jroc.Test262.Tests.language.expressions.Test_class.async_method_static.forbidden_ext.b1") { }

    [Fact(DisplayName = "cls-expr-async-meth-static-forbidden-ext-direct-access-prop-arguments.js")]
    public Task cls_expr_async_meth_static_forbidden_ext_direct_access_prop_arguments()
        => ExecutionTestFromFile("cls-expr-async-meth-static-forbidden-ext-direct-access-prop-arguments");

    [Fact(DisplayName = "cls-expr-async-meth-static-forbidden-ext-direct-access-prop-caller.js")]
    public Task cls_expr_async_meth_static_forbidden_ext_direct_access_prop_caller()
        => ExecutionTestFromFile("cls-expr-async-meth-static-forbidden-ext-direct-access-prop-caller");
}
