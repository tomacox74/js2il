using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.toLocaleString.BigInt;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArray.prototype.toLocaleString.BigInt") { }

    [Fact(DisplayName = "empty-instance-returns-empty-string.js")]
    public Task empty_instance_returns_empty_string() => ExecutionTestFromFile("empty-instance-returns-empty-string");

    [Fact(DisplayName = "get-length-uses-internal-arraylength.js")]
    public Task get_length_uses_internal_arraylength() => ExecutionTestFromFile("get-length-uses-internal-arraylength");

    [Fact(DisplayName = "return-result.js")]
    public Task return_result() => ExecutionTestFromFile("return-result");
}
