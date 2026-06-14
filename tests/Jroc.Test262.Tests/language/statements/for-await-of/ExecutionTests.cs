using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.statements.for_await_of;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.for_await_of") { }

    [Fact(DisplayName = "async-func-decl-dstr-array-rest-iteration")]
    public Task async_func_decl_dstr_array_rest_iteration()
        => ExecutionTest("async-func-decl-dstr-array-rest-iteration");

    [Fact(DisplayName = "async-func-decl-dstr-obj-empty-bool")]
    public Task async_func_decl_dstr_obj_empty_bool()
        => ExecutionTest("async-func-decl-dstr-obj-empty-bool");

    [Fact(DisplayName = "async-func-decl-dstr-obj-empty-num")]
    public Task async_func_decl_dstr_obj_empty_num()
        => ExecutionTest("async-func-decl-dstr-obj-empty-num");

    [Fact(DisplayName = "async-func-decl-dstr-obj-empty-obj")]
    public Task async_func_decl_dstr_obj_empty_obj()
        => ExecutionTest("async-func-decl-dstr-obj-empty-obj");

    [Fact(DisplayName = "async-func-decl-dstr-obj-empty-string")]
    public Task async_func_decl_dstr_obj_empty_string()
        => ExecutionTest("async-func-decl-dstr-obj-empty-string");

    [Fact(DisplayName = "async-func-dstr-let-obj-ptrn-empty")]
    public Task async_func_dstr_let_obj_ptrn_empty()
        => ExecutionTest("async-func-dstr-let-obj-ptrn-empty");

    [Fact(DisplayName = "head-lhs-async")]
    public Task head_lhs_async()
        => ExecutionTest("head-lhs-async");

}
