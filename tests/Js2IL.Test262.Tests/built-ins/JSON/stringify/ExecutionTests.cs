using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.JSON.stringify;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.JSON.stringify") { }

    [Fact(DisplayName = "builtin", Skip = "JSON.stringify intrinsic metadata support is incomplete.")]
    public Task builtin()
        => ExecutionTest("builtin");

    [Fact(DisplayName = "length", Skip = "JSON.stringify intrinsic metadata support is incomplete.")]
    public Task length()
        => ExecutionTest("length");

    [Fact(DisplayName = "name", Skip = "JSON.stringify intrinsic metadata support is incomplete.")]
    public Task name()
        => ExecutionTest("name");

    [Fact(DisplayName = "prop-desc", Skip = "JSON.stringify intrinsic metadata support is incomplete.")]
    public Task prop_desc()
        => ExecutionTest("prop-desc");
}
