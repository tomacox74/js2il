using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.ArrayBuffer.prototype.transferToFixedLength;

public class ImmutableBufferExecutionTests : DiskExecutionTestsBase
{
    public ImmutableBufferExecutionTests() : base("built_ins.ArrayBuffer.prototype.transferToFixedLength") { }

    [Fact(DisplayName = "this-is-immutable-arraybuffer.js")]
    public Task this_is_immutable_arraybuffer() => ExecutionTestFromFile("this-is-immutable-arraybuffer");
}
