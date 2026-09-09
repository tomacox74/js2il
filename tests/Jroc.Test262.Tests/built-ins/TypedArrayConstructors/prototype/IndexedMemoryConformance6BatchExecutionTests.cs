using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.prototype;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArrayConstructors.prototype") { }

    [Fact(DisplayName = "Symbol.iterator.js")]
    public Task Symbol_iterator() => ExecutionTestFromFile("Symbol.iterator");

    [Fact(DisplayName = "bigint-Symbol.iterator.js")]
    public Task bigint_Symbol_iterator() => ExecutionTestFromFile("bigint-Symbol.iterator");
}
