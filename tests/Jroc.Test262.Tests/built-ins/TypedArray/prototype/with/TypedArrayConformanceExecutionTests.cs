using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.with;

public sealed class TypedArrayConformanceExecutionTests : InMemoryExecutionTestsBase
{
    public TypedArrayConformanceExecutionTests() : base("TypedArray.Conformance") { }

    [Fact(DisplayName = "index-validated-against-current-length.js")]
    public Task index_validated_against_current_length()
        => ExecutionTestFromFile("index-validated-against-current-length");

    [Fact(DisplayName = "negative-index-resize-to-out-of-bounds.js")]
    public Task negative_index_resize_to_out_of_bounds()
        => ExecutionTestFromFile("negative-index-resize-to-out-of-bounds");

    [Fact(DisplayName = "valid-typedarray-index-checked-after-coercions.js")]
    public Task valid_typedarray_index_checked_after_coercions()
        => ExecutionTestFromFile("valid-typedarray-index-checked-after-coercions");

}
