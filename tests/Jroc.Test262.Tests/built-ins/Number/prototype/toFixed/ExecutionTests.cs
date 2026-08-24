using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Number.prototype.toFixed;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Number.prototype.toFixed") { }

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "return-type")]
    public Task return_type()
        => ExecutionTestFromFile("return-type");

    [Fact(DisplayName = "S15.7.4.5_A1.3_T01")]
    public Task S15_7_4_5_A1_3_T01()
        => ExecutionTestFromFile("S15.7.4.5_A1.3_T01");

    [Fact(DisplayName = "S15.7.4.5_A1.4_T01")]
    public Task S15_7_4_5_A1_4_T01()
        => ExecutionTestFromFile("S15.7.4.5_A1.4_T01");
}
