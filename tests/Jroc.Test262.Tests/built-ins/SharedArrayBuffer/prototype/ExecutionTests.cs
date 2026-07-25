using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.SharedArrayBuffer.prototype;

public sealed class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.SharedArrayBuffer.prototype") { }

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTest("constructor");

}
