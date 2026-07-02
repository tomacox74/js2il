using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.function.dstr;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.function.dstr") { }

    [Fact(DisplayName = "obj-ptrn-id-init-fn-name-gen")]
    public Task obj_ptrn_id_init_fn_name_gen()
        => ExecutionTest("obj-ptrn-id-init-fn-name-gen", allowUnhandledException: true);
}
