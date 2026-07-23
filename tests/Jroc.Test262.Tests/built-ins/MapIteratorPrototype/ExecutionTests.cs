using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.MapIteratorPrototype;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.MapIteratorPrototype") { }

    [Fact(DisplayName = "Symbol.toStringTag")]
    public Task Symbol_toStringTag()
        => ExecutionTestFromFile("Symbol.toStringTag");
}
