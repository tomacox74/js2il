using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.SharedArrayBuffer.prototype.maxByteLength;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.SharedArrayBuffer.prototype.maxByteLength") { }

    [Fact(DisplayName = "this-has-no-arraybufferdata-internal.js")]
    public Task this_has_no_arraybufferdata_internal() => ExecutionTestFromFile("this-has-no-arraybufferdata-internal");
}
