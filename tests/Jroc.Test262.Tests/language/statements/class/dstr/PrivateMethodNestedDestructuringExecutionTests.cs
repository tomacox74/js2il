using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements.class_.dstr;

public class PrivateMethodNestedDestructuringExecutionTests : FileSystemExecutionTestsBase
{
    public PrivateMethodNestedDestructuringExecutionTests()
        : base("language/statements/class/dstr", "language.statements.class_.dstr") { }

    [Fact(DisplayName = "private-meth-obj-ptrn-prop-ary-init.js")]
    public Task private_meth_obj_ptrn_prop_ary_init()
        => ExecutionTest("private-meth-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "private-meth-obj-ptrn-prop-ary.js")]
    public Task private_meth_obj_ptrn_prop_ary()
        => ExecutionTest("private-meth-obj-ptrn-prop-ary");

    [Fact(DisplayName = "private-meth-obj-ptrn-prop-obj-init.js")]
    public Task private_meth_obj_ptrn_prop_obj_init()
        => ExecutionTest("private-meth-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "private-meth-obj-ptrn-prop-obj.js")]
    public Task private_meth_obj_ptrn_prop_obj()
        => ExecutionTest("private-meth-obj-ptrn-prop-obj");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-prop-ary-init.js")]
    public Task private_meth_static_obj_ptrn_prop_ary_init()
        => ExecutionTest("private-meth-static-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-prop-ary.js")]
    public Task private_meth_static_obj_ptrn_prop_ary()
        => ExecutionTest("private-meth-static-obj-ptrn-prop-ary");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-prop-obj-init.js")]
    public Task private_meth_static_obj_ptrn_prop_obj_init()
        => ExecutionTest("private-meth-static-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "private-meth-static-obj-ptrn-prop-obj.js")]
    public Task private_meth_static_obj_ptrn_prop_obj()
        => ExecutionTest("private-meth-static-obj-ptrn-prop-obj");
}
