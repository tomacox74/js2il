using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView.prototype;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.DataView.prototype") { }

    [Fact(DisplayName = "Symbol.toStringTag.js")]
    public Task symbol_to_string_tag() => ExecutionTestFromFile("Symbol.toStringTag");
}
