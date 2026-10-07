using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype;

public sealed class TypedArrayConformanceExecutionTests : InMemoryExecutionTestsBase
{
    public TypedArrayConformanceExecutionTests() : base("TypedArray.Conformance") { }

    [Fact(DisplayName = "toString.js")]
    public Task toString()
        => ExecutionTestFromFile("toString");

}
