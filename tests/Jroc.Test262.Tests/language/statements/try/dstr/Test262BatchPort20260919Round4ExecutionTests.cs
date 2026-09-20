using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.statements.@try.dstr;

public class Test262BatchPort20260919Round4ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260919Round4ExecutionTests() : base("language.statements.try.dstr") { }

    [Fact(DisplayName = "ary-init-iter-close")]
    public Task ary_init_iter_close()
        => ExecutionTest("ary-init-iter-close");

    [Fact(DisplayName = "ary-init-iter-get-err-array-prototype")]
    public Task ary_init_iter_get_err_array_prototype()
        => ExecutionTest("ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "ary-init-iter-no-close")]
    public Task ary_init_iter_no_close()
        => ExecutionTest("ary-init-iter-no-close");

    [Fact(DisplayName = "ary-name-iter-val")]
    public Task ary_name_iter_val()
        => ExecutionTest("ary-name-iter-val");

    [Fact(DisplayName = "obj-init-null")]
    public Task obj_init_null()
        => ExecutionTest("obj-init-null");

}
