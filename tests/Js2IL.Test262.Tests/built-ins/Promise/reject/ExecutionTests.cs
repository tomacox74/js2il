using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Promise.reject;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Promise.reject") { }

    [Fact(DisplayName = "length", Skip = "Blocked by current Promise.reject metadata/descriptor surface.")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name", Skip = "Blocked by current Promise.reject metadata/descriptor surface.")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc", Skip = "Blocked by current Promise.reject metadata/descriptor surface.")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

}
