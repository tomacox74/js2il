namespace Jroc.Test262.Tests.language.expressions.generators.dstr;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.generators.dstr") { }

    [Fact(DisplayName = "dflt-obj-ptrn-prop-ary-init.js")]
    public Task dflt_obj_ptrn_prop_ary_init()
        => ExecutionTest("dflt-obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-ary.js")]
    public Task dflt_obj_ptrn_prop_ary()
        => ExecutionTest("dflt-obj-ptrn-prop-ary");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-eval-err.js")]
    public Task dflt_obj_ptrn_prop_eval_err()
        => ExecutionTest("dflt-obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-obj-init.js")]
    public Task dflt_obj_ptrn_prop_obj_init()
        => ExecutionTest("dflt-obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "dflt-obj-ptrn-prop-obj.js")]
    public Task dflt_obj_ptrn_prop_obj()
        => ExecutionTest("dflt-obj-ptrn-prop-obj");

    [Fact(DisplayName = "obj-ptrn-prop-ary-init.js")]
    public Task obj_ptrn_prop_ary_init()
        => ExecutionTest("obj-ptrn-prop-ary-init");

    [Fact(DisplayName = "obj-ptrn-prop-ary.js")]
    public Task obj_ptrn_prop_ary()
        => ExecutionTest("obj-ptrn-prop-ary");

    [Fact(DisplayName = "obj-ptrn-prop-eval-err.js")]
    public Task obj_ptrn_prop_eval_err()
        => ExecutionTest("obj-ptrn-prop-eval-err");

    [Fact(DisplayName = "obj-ptrn-prop-obj-init.js")]
    public Task obj_ptrn_prop_obj_init()
        => ExecutionTest("obj-ptrn-prop-obj-init");

    [Fact(DisplayName = "obj-ptrn-prop-obj.js")]
    public Task obj_ptrn_prop_obj()
        => ExecutionTest("obj-ptrn-prop-obj");
}
