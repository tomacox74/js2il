using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.sort.BigInt;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.prototype.sort.BigInt") { }

    [Fact(DisplayName = "comparefn-call-throws.js")]
    public Task comparefn_call_throws() => ExecutionTestFromFile("comparefn-call-throws");

    [Fact(DisplayName = "comparefn-is-undefined.js")]
    public Task comparefn_is_undefined() => ExecutionTestFromFile("comparefn-is-undefined");

    [Fact(DisplayName = "comparefn-nonfunction-call-throws.js")]
    public Task comparefn_nonfunction_call_throws() => ExecutionTestFromFile("comparefn-nonfunction-call-throws");

    [Fact(DisplayName = "return-same-instance.js")]
    public Task return_same_instance() => ExecutionTestFromFile("return-same-instance");

    [Fact(DisplayName = "sortcompare-with-no-tostring.js")]
    public Task sortcompare_with_no_tostring() => ExecutionTestFromFile("sortcompare-with-no-tostring");
}
