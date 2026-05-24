using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Promise.prototype.then;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Promise.prototype.then") { }

    [Fact(DisplayName = "length", Skip = "Blocked by current Promise.prototype.then length/name metadata surface.")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name", Skip = "Blocked by current Promise.prototype.then length/name metadata surface.")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

}
