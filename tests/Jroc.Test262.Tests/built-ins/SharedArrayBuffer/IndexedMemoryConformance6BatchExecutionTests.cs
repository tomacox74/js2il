using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.SharedArrayBuffer;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.SharedArrayBuffer") { }

    [Fact(DisplayName = "options-maxbytelength-data-allocation-after-object-creation.js")]
    public Task options_maxbytelength_data_allocation_after_object_creation() => ExecutionTestFromFile("options-maxbytelength-data-allocation-after-object-creation");

    [Fact(DisplayName = "options-maxbytelength-undefined.js")]
    public Task options_maxbytelength_undefined() => ExecutionTestFromFile("options-maxbytelength-undefined");

    [Fact(DisplayName = "options-non-object.js")]
    public Task options_non_object() => ExecutionTestFromFile("options-non-object");

    [Fact(DisplayName = "prototype-from-newtarget.js")]
    public Task prototype_from_newtarget() => ExecutionTestFromFile("prototype-from-newtarget");

    [Fact(DisplayName = "return-abrupt-from-length-symbol.js")]
    public Task return_abrupt_from_length_symbol() => ExecutionTestFromFile("return-abrupt-from-length-symbol");
}
