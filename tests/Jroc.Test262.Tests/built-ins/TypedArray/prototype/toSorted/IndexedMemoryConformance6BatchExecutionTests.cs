using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.toSorted;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.prototype.toSorted") { }

    [Fact(DisplayName = "ignores-species.js")]
    public Task ignores_species() => ExecutionTestFromFile("ignores-species");

    [Fact(DisplayName = "length-property-ignored.js")]
    public Task length_property_ignored() => ExecutionTestFromFile("length-property-ignored");

    [Fact(DisplayName = "property-descriptor.js")]
    public Task property_descriptor() => ExecutionTestFromFile("property-descriptor");
}
