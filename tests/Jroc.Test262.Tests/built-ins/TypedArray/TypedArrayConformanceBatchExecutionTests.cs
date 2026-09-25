using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray;

public class TypedArrayConformanceBatchExecutionTests : InMemoryExecutionTestsBase
{
    public TypedArrayConformanceBatchExecutionTests() : base("built_ins.TypedArray") { }

    [Fact(DisplayName = "invoked.js")]
    public Task invoked() => ExecutionTestFromFile("invoked");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

}
