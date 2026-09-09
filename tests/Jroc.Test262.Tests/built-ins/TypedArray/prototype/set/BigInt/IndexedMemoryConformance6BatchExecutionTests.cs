using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.@set.BigInt;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.prototype.set.BigInt") { }

    [Fact(DisplayName = "typedarray-arg-set-values-diff-buffer-other-type-sab.js")]
    public Task typedarray_arg_set_values_diff_buffer_other_type_sab() => ExecutionTestFromFile("typedarray-arg-set-values-diff-buffer-other-type-sab");

    [Fact(DisplayName = "typedarray-arg-set-values-diff-buffer-same-type-sab.js")]
    public Task typedarray_arg_set_values_diff_buffer_same_type_sab() => ExecutionTestFromFile("typedarray-arg-set-values-diff-buffer-same-type-sab");

    [Fact(DisplayName = "typedarray-arg-set-values-same-buffer-same-type-sab.js")]
    public Task typedarray_arg_set_values_same_buffer_same_type_sab() => ExecutionTestFromFile("typedarray-arg-set-values-same-buffer-same-type-sab");
}
