using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.fromCharCode;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.fromCharCode") { }

    [Fact(DisplayName = "S9.7_A2.1.js")]
    public Task S9_7_A2_1()
        => ExecutionTestFromFile("S9.7_A2.1");

    [Fact(DisplayName = "touint16-tonumber-throws-bigint.js")]
    public Task touint16_tonumber_throws_bigint()
        => ExecutionTestFromFile("touint16-tonumber-throws-bigint");

    [Fact(DisplayName = "touint16-tonumber-throws-valueof.js")]
    public Task touint16_tonumber_throws_valueof()
        => ExecutionTestFromFile("touint16-tonumber-throws-valueof");
}
