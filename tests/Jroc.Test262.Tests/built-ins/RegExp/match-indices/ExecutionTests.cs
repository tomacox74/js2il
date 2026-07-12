using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.match_indices;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExp.match-indices") { }

    [Fact(DisplayName = "indices-array-unmatched.js")]
    public Task indices_array_unmatched()
        => ExecutionTestFromFile("indices-array-unmatched");

    [Fact(DisplayName = "indices-groups-object-undefined.js")]
    public Task indices_groups_object_undefined()
        => ExecutionTestFromFile("indices-groups-object-undefined");

    [Fact(DisplayName = "indices-groups-properties.js")]
    public Task indices_groups_properties()
        => ExecutionTestFromFile("indices-groups-properties");
}
