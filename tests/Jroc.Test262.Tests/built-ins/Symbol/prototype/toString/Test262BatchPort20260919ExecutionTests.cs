using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Symbol.prototype.toString;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Symbol.prototype.toString") { }

    [Fact(DisplayName = "undefined")]
    public Task undefined()
        => ExecutionTestFromFile("undefined");

}
