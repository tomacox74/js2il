using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.SharedArrayBuffer;

public class Port400ExecutionTests : InMemoryExecutionTestsBase
{
    public Port400ExecutionTests() : base("built_ins.SharedArrayBuffer") { }

    [Fact(DisplayName = "data-allocation-after-object-creation.js")]
    public Task Port400_data_allocation_after_object_creation()
        => ExecutionTestFromFile("data-allocation-after-object-creation");

    [Fact(DisplayName = "toindex-length.js")]
    public Task Port400_toindex_length()
        => ExecutionTestFromFile("toindex-length");

    [Fact(DisplayName = "undefined-newtarget-throws.js")]
    public Task Port400_undefined_newtarget_throws()
        => ExecutionTestFromFile("undefined-newtarget-throws");
}
