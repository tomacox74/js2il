using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.SharedArrayBuffer.prototype.growable;

public sealed class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.SharedArrayBuffer.prototype.growable") { }

    [Fact(DisplayName = "return-growable.js")]
    public Task ported_return_growable() => ExecutionTest("return-growable");
}
