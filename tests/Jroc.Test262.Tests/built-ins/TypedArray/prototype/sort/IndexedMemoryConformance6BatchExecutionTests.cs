using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.sort;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.prototype.sort") { }

    [Fact(DisplayName = "comparefn-nonfunction-call-throws.js")]
    public Task comparefn_nonfunction_call_throws() => ExecutionTestFromFile("comparefn-nonfunction-call-throws");
}
