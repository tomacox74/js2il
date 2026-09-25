using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakMap.prototype;

public class PortAdditionalExecutionTests : InMemoryExecutionTestsBase
{
    public PortAdditionalExecutionTests() : base("built_ins.WeakMap.prototype") { }

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");

}
