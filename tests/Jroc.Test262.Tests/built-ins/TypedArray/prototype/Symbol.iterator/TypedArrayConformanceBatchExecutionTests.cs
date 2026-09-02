using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.Symbol.iterator;

public class TypedArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.Symbol.iterator") { }

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

}
