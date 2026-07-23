using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.Symbol.toStringTag;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Set.prototype.Symbol.toStringTag") { }

    [Fact(DisplayName = "property-descriptor")]
    public Task property_descriptor()
        => ExecutionTestFromFile("property-descriptor");

    [Fact(DisplayName = "Symbol.toStringTag")]
    public Task Symbol_toStringTag()
        => ExecutionTestFromFile("Symbol.toStringTag");
}
