using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.named_groups;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExp.named-groups") { }

    [Fact(DisplayName = "groups-object-undefined.js")]
    public Task groups_object_undefined()
        => ExecutionTestFromFile("groups-object-undefined");

    [Fact(DisplayName = "groups-properties.js")]
    public Task groups_properties()
        => ExecutionTestFromFile("groups-properties");
}
