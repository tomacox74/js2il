using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Map.prototype;

public class PortAdditionalExecutionTests : DiskExecutionTestsBase
{
    public PortAdditionalExecutionTests() : base("built_ins.Map.prototype") { }

    [Fact(DisplayName = "Symbol.iterator")]
    public Task Symbol_iterator()
        => ExecutionTestFromFile("Symbol.iterator");

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");

}
