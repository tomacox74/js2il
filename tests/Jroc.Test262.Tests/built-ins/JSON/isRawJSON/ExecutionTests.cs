using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.JSON.isRawJSON;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.JSON.isRawJSON") { }

    [Fact(DisplayName = "basic.js")]
    public Task basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "builtin.js")]
    public Task builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");
}
