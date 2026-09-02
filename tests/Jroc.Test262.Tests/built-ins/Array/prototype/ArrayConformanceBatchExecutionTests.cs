using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype;

public class ArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public ArrayConformanceBatchExecutionTests() : base("built_ins.Array.prototype") { }

    [Fact(DisplayName = "Symbol.iterator.js")]
    public Task Symbol_iterator() => ExecutionTestFromFile("Symbol.iterator");

    [Fact(DisplayName = "constructor.js")]
    public Task constructor() => ExecutionTestFromFile("constructor");

}
