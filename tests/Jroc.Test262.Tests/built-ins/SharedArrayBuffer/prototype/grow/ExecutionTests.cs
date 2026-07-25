using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.SharedArrayBuffer.prototype.grow;

public sealed class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.SharedArrayBuffer.prototype.grow") { }

    [Fact(DisplayName = "this-is-sharedarraybuffer")]
    public Task this_is_sharedarraybuffer()
        => ExecutionTest("this-is-sharedarraybuffer");

}
