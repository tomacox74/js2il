using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.expressions.class_.dstr;

public class PrivateMethodNestedDestructuringExecutionTests : FileSystemExecutionTestsBase
{
    public PrivateMethodNestedDestructuringExecutionTests()
        : base("language/expressions/class/dstr", "language.expressions.class_.dstr") { }

    [Fact(DisplayName = "private-meth-static-obj-ptrn-prop-ary-init.js")]
    public Task private_meth_static_obj_ptrn_prop_ary_init()
        => ExecutionTest("private-meth-static-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-prop-ary.js")]
    public Task private_meth_static_obj_ptrn_prop_ary()
        => ExecutionTest("private-meth-static-obj-ptrn-prop-ary");
}
