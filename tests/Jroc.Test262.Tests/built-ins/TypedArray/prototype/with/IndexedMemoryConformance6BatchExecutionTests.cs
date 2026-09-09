using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.@with;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.prototype.with") { }

    [Fact(DisplayName = "negative-index-resize-to-in-bounds.js")]
    public Task negative_index_resize_to_in_bounds() => ExecutionTestFromFile("negative-index-resize-to-in-bounds");

    [Fact(DisplayName = "property-descriptor.js")]
    public Task property_descriptor() => ExecutionTestFromFile("property-descriptor");

    [Fact(DisplayName = "this-value-invalid.js")]
    public Task this_value_invalid() => ExecutionTestFromFile("this-value-invalid");
}
