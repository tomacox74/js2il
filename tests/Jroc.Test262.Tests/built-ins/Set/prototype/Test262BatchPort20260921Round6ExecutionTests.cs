using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype;

public class Test262BatchPort20260921Round6ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260921Round6ExecutionTests() : base("built_ins.Set.prototype") { }

    [Fact(DisplayName = "Symbol.toStringTag")]
    public Task Symbol_toStringTag()
        => ExecutionTestFromFile("Symbol.toStringTag");

}
