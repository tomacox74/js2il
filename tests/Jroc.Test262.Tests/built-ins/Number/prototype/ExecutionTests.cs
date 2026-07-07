using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Number.prototype;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Number.prototype") { }

    [Fact(DisplayName = "15.7.3.1-2")]
    public Task _15_7_3_1_2()
        => ExecutionTestFromFile("15.7.3.1-2");

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "S15.7.4_A1")]
    public Task S15_7_4_A1()
        => ExecutionTestFromFile("S15.7.4_A1");

    [Fact(DisplayName = "S15.7.4_A2")]
    public Task S15_7_4_A2()
        => ExecutionTestFromFile("S15.7.4_A2");

    [Fact(DisplayName = "S15.7.4_A3.1")]
    public Task S15_7_4_A3_1()
        => ExecutionTestFromFile("S15.7.4_A3.1");
}
