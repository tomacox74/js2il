using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.String.prototype.charAt;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.charAt") { }

    [Fact(DisplayName = "pos-coerce-string")]
    public Task pos_coerce_string()
        => ExecutionTestFromFile("pos-coerce-string");

    [Fact(DisplayName = "pos-rounding")]
    public Task pos_rounding()
        => ExecutionTestFromFile("pos-rounding");

    [Fact(DisplayName = "S15.5.4.4_A2")]
    public Task S15_5_4_4_A2()
        => ExecutionTestFromFile("S15.5.4.4_A2");

    [Fact(DisplayName = "S9.4_A1")]
    public Task S9_4_A1()
        => ExecutionTestFromFile("S9.4_A1");
}
