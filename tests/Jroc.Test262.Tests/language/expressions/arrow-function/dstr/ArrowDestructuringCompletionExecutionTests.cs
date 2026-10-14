namespace Jroc.Test262.Tests.language.expressions.arrow_function.dstr;

public class ArrowDestructuringCompletionExecutionTests : DiskExecutionTestsBase
{
    public ArrowDestructuringCompletionExecutionTests() : base("language.expressions.arrow_function.dstr") { }

    [Fact(DisplayName = "obj-ptrn-prop-ary-init.js")]
    public Task obj_ptrn_prop_ary_init() => ExecutionTest("obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "obj-ptrn-prop-ary.js")]
    public Task obj_ptrn_prop_ary() => ExecutionTest("obj-ptrn-prop-ary");

    [Fact(DisplayName = "obj-ptrn-prop-obj-init.js")]
    public Task obj_ptrn_prop_obj_init() => ExecutionTest("obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "obj-ptrn-prop-obj.js")]
    public Task obj_ptrn_prop_obj() => ExecutionTest("obj-ptrn-prop-obj");
}
