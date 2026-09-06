using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.reverse.BigInt;

public class TypedArraySecondFollowUpConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArraySecondFollowUpConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.reverse.BigInt") { }

    [Fact(DisplayName = "get-length-uses-internal-arraylength.js")]
    public Task get_length_uses_internal_arraylength() => ExecutionTestFromFile("get-length-uses-internal-arraylength");

    [Fact(DisplayName = "preserves-non-numeric-properties.js")]
    public Task preserves_non_numeric_properties() => ExecutionTestFromFile("preserves-non-numeric-properties");

    [Fact(DisplayName = "returns-original-object.js")]
    public Task returns_original_object() => ExecutionTestFromFile("returns-original-object");

}
