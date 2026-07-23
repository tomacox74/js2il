using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.SetIteratorPrototype;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.SetIteratorPrototype") { }

    [Fact(DisplayName = "Symbol.toStringTag")]
    public Task Symbol_toStringTag()
        => ExecutionTestFromFile("Symbol.toStringTag");
}
