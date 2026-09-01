namespace Jroc.Test262.Tests.built_ins.String.prototype;

public partial class ExecutionTests
{
    [Fact(DisplayName = "S15.5.3.1_A1.js")]
    public Task S15_5_3_1_A1() => ExecutionTestFromFile("S15.5.3.1_A1");
    [Fact(DisplayName = "S15.5.3.1_A2.js")]
    public Task S15_5_3_1_A2() => ExecutionTestFromFile("S15.5.3.1_A2");
    [Fact(DisplayName = "S15.5.3.1_A3.js")]
    public Task S15_5_3_1_A3() => ExecutionTestFromFile("S15.5.3.1_A3");
    [Fact(DisplayName = "S15.5.3.1_A4.js")]
    public Task S15_5_3_1_A4() => ExecutionTestFromFile("S15.5.3.1_A4");
}
