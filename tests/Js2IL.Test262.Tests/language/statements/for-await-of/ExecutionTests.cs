using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.statements.for_await_of;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.for_await_of") { }

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-empty")]
    public Task async_func_dstr_let_obj_ptrn_empty()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-empty");

    [Fact(DisplayName = "head-lhs-async")]
    public Task head_lhs_async()
        => ExecutionTest("head-lhs-async");

}
