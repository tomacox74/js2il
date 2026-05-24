using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Promise.race;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Promise.race") { }

    [Fact(DisplayName = "length", Skip = "Blocked by current Promise.race metadata/descriptor surface.")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name", Skip = "Blocked by current Promise.race metadata/descriptor surface.")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc", Skip = "Blocked by current Promise.race metadata/descriptor surface.")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

}
