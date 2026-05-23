using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.statements.for_of;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.for_of") { }

    [Fact(DisplayName = "ArgumentsObject_mapped-aliasing")]
    public Task ArgumentsObject_mapped_aliasing()
        => ExecutionTest("ArgumentsObject_mapped-aliasing");

    [Fact(DisplayName = "ArgumentsObject_mapped")]
    public Task ArgumentsObject_mapped()
        => ExecutionTest("ArgumentsObject_mapped");

    [Fact(DisplayName = "ArgumentsObject_unmapped-aliasing")]
    public Task ArgumentsObject_unmapped_aliasing()
        => ExecutionTest("ArgumentsObject_unmapped-aliasing");

    [Fact(DisplayName = "Array.prototype.Symbol.iterator")]
    public Task Array_prototype_Symbol_iterator()
        => ExecutionTest("Array.prototype.Symbol.iterator");

    [Fact(DisplayName = "Array.prototype.entries")]
    public Task Array_prototype_entries()
        => ExecutionTest("Array.prototype.entries");

    [Fact(DisplayName = "Array.prototype.keys")]
    public Task Array_prototype_keys()
        => ExecutionTest("Array.prototype.keys");

    [Fact(DisplayName = "arguments-mapped-aliasing")]
    public Task arguments_mapped_aliasing()
        => ExecutionTest("arguments-mapped-aliasing");

    [Fact(DisplayName = "arguments-mapped-mutation")]
    public Task arguments_mapped_mutation()
        => ExecutionTest("arguments-mapped-mutation");

    [Fact(DisplayName = "arguments-mapped")]
    public Task arguments_mapped()
        => ExecutionTest("arguments-mapped");

    [Fact(DisplayName = "arguments-unmapped-aliasing")]
    public Task arguments_unmapped_aliasing()
        => ExecutionTest("arguments-unmapped-aliasing");

    [Fact(DisplayName = "arguments-unmapped-mutation")]
    public Task arguments_unmapped_mutation()
        => ExecutionTest("arguments-unmapped-mutation");

    [Fact(DisplayName = "arguments-unmapped")]
    public Task arguments_unmapped()
        => ExecutionTest("arguments-unmapped");

    [Fact(DisplayName = "array-contract-expand")]
    public Task array_contract_expand()
        => ExecutionTest("array-contract-expand");

    [Fact(DisplayName = "array-contract")]
    public Task array_contract()
        => ExecutionTest("array-contract");

    [Fact(DisplayName = "array-expand-contract")]
    public Task array_expand_contract()
        => ExecutionTest("array-expand-contract");

    [Fact(DisplayName = "array-expand")]
    public Task array_expand()
        => ExecutionTest("array-expand");

    [Fact(DisplayName = "array-key-get-error")]
    public Task array_key_get_error()
        => ExecutionTest("array-key-get-error");

    [Fact(DisplayName = "array")]
    public Task array()
        => ExecutionTest("array");

    [Fact(DisplayName = "body-dstr-assign-error")]
    public Task body_dstr_assign_error()
        => ExecutionTest("body-dstr-assign-error");

    [Fact(DisplayName = "body-dstr-assign")]
    public Task body_dstr_assign()
        => ExecutionTest("body-dstr-assign");

    [Fact(DisplayName = "array-elem-init-assignment", Skip = "Tracked by #1093: currently fails under Js2IL.")]
    public Task dstr_array_elem_init_assignment()
        => ExecutionTest(@"dstr\array-elem-init-assignment");

    [Fact(DisplayName = "array-elem-init-in", Skip = "Tracked by #1093: currently fails under Js2IL.")]
    public Task dstr_array_elem_init_in()
        => ExecutionTest(@"dstr\array-elem-init-in");

    [Fact(DisplayName = "array-elem-init-let", Skip = "Tracked by #1093: currently fails under Js2IL.")]
    public Task dstr_array_elem_init_let()
        => ExecutionTest(@"dstr\array-elem-init-let");

    [Fact(DisplayName = "array-elem-init-order", Skip = "Tracked by #1093: currently fails under Js2IL.")]
    public Task dstr_array_elem_init_order()
        => ExecutionTest(@"dstr\array-elem-init-order");

    [Fact(DisplayName = "array-elem-iter-nrml-close-null", Skip = "Tracked by #1093: currently fails under Js2IL.")]
    public Task dstr_array_elem_iter_nrml_close_null()
        => ExecutionTest(@"dstr\array-elem-iter-nrml-close-null");

    [Fact(DisplayName = "array-elem-iter-nrml-close-skip", Skip = "Tracked by #1093: currently fails under Js2IL.")]
    public Task dstr_array_elem_iter_nrml_close_skip()
        => ExecutionTest(@"dstr\array-elem-iter-nrml-close-skip");

    [Fact(DisplayName = "array-elem-iter-nrml-close", Skip = "Tracked by #1093: currently fails under Js2IL.")]
    public Task dstr_array_elem_iter_nrml_close()
        => ExecutionTest(@"dstr\array-elem-iter-nrml-close");

    [Fact(DisplayName = "array-elem-iter-rtrn-close-null", Skip = "Tracked by #1093: currently fails under Js2IL.")]
    public Task dstr_array_elem_iter_rtrn_close_null()
        => ExecutionTest(@"dstr\array-elem-iter-rtrn-close-null");

    [Fact(DisplayName = "float32array-mutate", Skip = "Tracked by #1093: JS2IL does not yet pass this advanced test262 scenario.")]
    public Task float32array_mutate()
        => ExecutionTest("float32array-mutate");

    [Fact(DisplayName = "float32array", Skip = "Tracked by #1093: JS2IL does not yet pass this advanced test262 scenario.")]
    public Task float32array()
        => ExecutionTest("float32array");

    [Fact(DisplayName = "float64array-mutate")]
    public Task float64array_mutate()
        => ExecutionTest("float64array-mutate");

    [Fact(DisplayName = "float64array")]
    public Task float64array()
        => ExecutionTest("float64array");

    [Fact(DisplayName = "generator")]
    public Task generator()
        => ExecutionTest("generator");

    [Fact(DisplayName = "generic-iterable")]
    public Task generic_iterable()
        => ExecutionTest("generic-iterable");

    [Fact(DisplayName = "int16array-mutate", Skip = "Tracked by #1093: JS2IL does not yet pass this advanced test262 scenario.")]
    public Task int16array_mutate()
        => ExecutionTest("int16array-mutate");

    [Fact(DisplayName = "int16array", Skip = "Tracked by #1093: JS2IL does not yet pass this advanced test262 scenario.")]
    public Task int16array()
        => ExecutionTest("int16array");

    [Fact(DisplayName = "int32array-mutate")]
    public Task int32array_mutate()
        => ExecutionTest("int32array-mutate");

    [Fact(DisplayName = "int32array")]
    public Task int32array()
        => ExecutionTest("int32array");

    [Fact(DisplayName = "int8array", Skip = "Tracked by #1093: JS2IL does not yet pass this advanced test262 scenario.")]
    public Task int8array()
        => ExecutionTest("int8array");

    [Fact(DisplayName = "iterator-as-proxy")]
    public Task iterator_as_proxy()
        => ExecutionTest("iterator-as-proxy");

    [Fact(DisplayName = "iterator-next-result-done-attr")]
    public Task iterator_next_result_done_attr()
        => ExecutionTest("iterator-next-result-done-attr");
}
