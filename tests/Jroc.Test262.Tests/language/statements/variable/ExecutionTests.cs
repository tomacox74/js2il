using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements.variable;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\statements\variable", "language.statements.variable") { }

    [Fact(DisplayName = "S12.2_A2")]
    public Task S12_2_A2()
        => ExecutionTest("S12.2_A2");

    [Fact(DisplayName = "S12.2_A3")]
    public Task S12_2_A3()
        => ExecutionTest("S12.2_A3");

    [Fact(DisplayName = "S12.2_A4")]
    public Task S12_2_A4()
        => ExecutionTest("S12.2_A4");

    [Fact(DisplayName = "arguments-non-strict")]
    public Task arguments_non_strict()
        => ExecutionTest("arguments-non-strict");

    [Fact(DisplayName = "binding-resolution")]
    public Task binding_resolution()
        => ExecutionTest("binding-resolution");

    [Fact(DisplayName = "eval-non-strict")]
    public Task eval_non_strict()
        => ExecutionTest("eval-non-strict");
    [Fact(DisplayName = "ary-init-iter-close")]
    public Task dstr_ary_init_iter_close()
        => ExecutionTest(@"dstr\ary-init-iter-close");

    [Fact(DisplayName = "ary-init-iter-get-err-array-prototype")]
    public Task dstr_ary_init_iter_get_err_array_prototype()
        => ExecutionTest(@"dstr\ary-init-iter-get-err-array-prototype");

    [Fact(DisplayName = "ary-init-iter-no-close")]
    public Task dstr_ary_init_iter_no_close()
        => ExecutionTest(@"dstr\ary-init-iter-no-close");

    [Fact(DisplayName = "ary-name-iter-val")]
    public Task dstr_ary_name_iter_val()
        => ExecutionTest(@"dstr\ary-name-iter-val");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elem-init")]
    public Task dstr_ary_ptrn_elem_ary_elem_init()
        => ExecutionTest(@"dstr\ary-ptrn-elem-ary-elem-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elem-iter")]
    public Task dstr_ary_ptrn_elem_ary_elem_iter()
        => ExecutionTest(@"dstr\ary-ptrn-elem-ary-elem-iter");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elision-init")]
    public Task dstr_ary_ptrn_elem_ary_elision_init()
        => ExecutionTest(@"dstr\ary-ptrn-elem-ary-elision-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-elision-iter")]
    public Task dstr_ary_ptrn_elem_ary_elision_iter()
        => ExecutionTest(@"dstr\ary-ptrn-elem-ary-elision-iter");

    [Fact(DisplayName = "ary-ptrn-elem-ary-empty-init")]
    public Task dstr_ary_ptrn_elem_ary_empty_init()
        => ExecutionTest(@"dstr\ary-ptrn-elem-ary-empty-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-empty-iter")]
    public Task dstr_ary_ptrn_elem_ary_empty_iter()
        => ExecutionTest(@"dstr\ary-ptrn-elem-ary-empty-iter");

    [Fact(DisplayName = "ary-ptrn-elem-ary-rest-init")]
    public Task dstr_ary_ptrn_elem_ary_rest_init()
        => ExecutionTest(@"dstr\ary-ptrn-elem-ary-rest-init");

    [Fact(DisplayName = "ary-ptrn-elem-ary-rest-iter")]
    public Task dstr_ary_ptrn_elem_ary_rest_iter()
        => ExecutionTest(@"dstr\ary-ptrn-elem-ary-rest-iter");

}
