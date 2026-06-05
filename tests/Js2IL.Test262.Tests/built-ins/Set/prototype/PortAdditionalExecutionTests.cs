using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Set.prototype;

public class PortAdditionalExecutionTests : DiskExecutionTestsBase
{
    public PortAdditionalExecutionTests() : base("built_ins.Set.prototype") { }

    [Fact(DisplayName = "Symbol.iterator")]
    public Task Symbol_iterator()
        => ExecutionTestFromFile("Symbol.iterator");

}
