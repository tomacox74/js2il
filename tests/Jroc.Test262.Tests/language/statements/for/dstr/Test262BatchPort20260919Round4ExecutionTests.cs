using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.statements.@for.dstr;

public class Test262BatchPort20260919Round4ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260919Round4ExecutionTests() : base("language.statements.for.dstr") { }

    [Fact(DisplayName = "const-ary-init-iter-close")]
    public Task const_ary_init_iter_close()
        => ExecutionTest("const-ary-init-iter-close");

    [Fact(DisplayName = "const-ary-init-iter-get-err-array-prototype")]
    public Task const_ary_init_iter_get_err_array_prototype()
        => ExecutionTest("const-ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "const-ary-init-iter-no-close")]
    public Task const_ary_init_iter_no_close()
        => ExecutionTest("const-ary-init-iter-no-close");

    [Fact(DisplayName = "const-ary-name-iter-val")]
    public Task const_ary_name_iter_val()
        => ExecutionTest("const-ary-name-iter-val");

    [Fact(DisplayName = "const-ary-ptrn-elem-ary-elem-init")]
    public Task const_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest("const-ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "const-ary-ptrn-elem-ary-elem-iter")]
    public Task const_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest("const-ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "const-ary-ptrn-elem-ary-elision-init")]
    public Task const_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest("const-ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "const-ary-ptrn-elem-ary-elision-iter")]
    public Task const_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest("const-ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "const-ary-ptrn-elem-ary-empty-init")]
    public Task const_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest("const-ary-ptrn-elem-ary-empty-init");

}
