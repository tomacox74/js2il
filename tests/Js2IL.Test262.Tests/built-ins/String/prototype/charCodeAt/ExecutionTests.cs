using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.String.prototype.charCodeAt;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.charCodeAt") { }

    [Fact(DisplayName = "pos-coerce-string")]
    public Task pos_coerce_string()
        => ExecutionTestFromFile("pos-coerce-string");

    [Fact(DisplayName = "pos-rounding")]
    public Task pos_rounding()
        => ExecutionTestFromFile("pos-rounding");

    [Fact(DisplayName = "S15.5.4.5_A2")]
    public Task S15_5_4_5_A2()
        => ExecutionTestFromFile("S15.5.4.5_A2");

    [Fact(DisplayName = "S15.5.4.5_A3")]
    public Task S15_5_4_5_A3()
        => ExecutionTestFromFile("S15.5.4.5_A3");
}
