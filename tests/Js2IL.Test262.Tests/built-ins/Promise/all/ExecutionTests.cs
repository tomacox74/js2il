using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Promise.all;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Promise.all") { }

    [Fact(DisplayName = "length", Skip = "Blocked by current Promise.all metadata/descriptor surface.")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name", Skip = "Blocked by current Promise.all metadata/descriptor surface.")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc", Skip = "Blocked by current Promise.all metadata/descriptor surface.")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

}
