using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.SharedArrayBuffer.prototype;

public sealed class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.SharedArrayBuffer.prototype") { }

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTest("constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc()
        => ExecutionTest("prop-desc");

    [Fact(DisplayName = "Symbol.toStringTag.js")]
    public Task symbol_to_string_tag()
        => ExecutionTest("Symbol.toStringTag");
}
