using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Reflect;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Reflect") { }

    [Fact(DisplayName = "Symbol.toStringTag")]
    public Task Symbol_toStringTag()
        => ExecutionTestFromFile("Symbol.toStringTag");
}
