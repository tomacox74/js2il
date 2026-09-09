using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.map.BigInt;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.prototype.map.BigInt") { }

    [Fact(DisplayName = "callbackfn-return-does-not-change-instance.js")]
    public Task callbackfn_return_does_not_change_instance() => ExecutionTestFromFile("callbackfn-return-does-not-change-instance");

    [Fact(DisplayName = "return-abrupt-from-this-out-of-bounds.js")]
    public Task return_abrupt_from_this_out_of_bounds() => ExecutionTestFromFile("return-abrupt-from-this-out-of-bounds");

    [Fact(DisplayName = "speciesctor-destination-resizable.js")]
    public Task speciesctor_destination_resizable() => ExecutionTestFromFile("speciesctor-destination-resizable");

    [Fact(DisplayName = "speciesctor-get-species-custom-ctor-length-throws-resizable-arraybuffer.js")]
    public Task speciesctor_get_species_custom_ctor_length_throws_resizable_arraybuffer() => ExecutionTestFromFile("speciesctor-get-species-custom-ctor-length-throws-resizable-arraybuffer");

    [Fact(DisplayName = "values-are-not-cached.js")]
    public Task values_are_not_cached() => ExecutionTestFromFile("values-are-not-cached");
}
