using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.toReversed;

public sealed class TypedArrayConformanceExecutionTests : InMemoryExecutionTestsBase
{
    public TypedArrayConformanceExecutionTests() : base("TypedArray.Conformance") { }

    [Fact(DisplayName = "this-value-invalid.js")]
    public Task this_value_invalid()
        => ExecutionTestFromFile("this-value-invalid");

}
