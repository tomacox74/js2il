using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.prototype;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExp.prototype") { }

    [Fact(DisplayName = "15.10.6.js")]
    public Task _15_10_6()
        => ExecutionTestFromFile("15.10.6");

    [Fact(DisplayName = "S15.10.5.1_A1.js")]
    public Task S15_10_5_1_A1()
        => ExecutionTestFromFile("S15.10.5.1_A1");

    [Fact(DisplayName = "S15.10.5.1_A2.js")]
    public Task S15_10_5_1_A2()
        => ExecutionTestFromFile("S15.10.5.1_A2");

    [Fact(DisplayName = "S15.10.5.1_A3.js")]
    public Task S15_10_5_1_A3()
        => ExecutionTestFromFile("S15.10.5.1_A3");

    [Fact(DisplayName = "S15.10.5.1_A4.js")]
    public Task S15_10_5_1_A4()
        => ExecutionTestFromFile("S15.10.5.1_A4");

    [Fact(DisplayName = "S15.10.6.1_A1_T1.js")]
    public Task S15_10_6_1_A1_T1()
        => ExecutionTestFromFile("S15.10.6.1_A1_T1");

    [Fact(DisplayName = "S15.10.6.1_A1_T2.js")]
    public Task S15_10_6_1_A1_T2()
        => ExecutionTestFromFile("S15.10.6.1_A1_T2");

    [Fact(DisplayName = "S15.10.6_A1_T1.js")]
    public Task S15_10_6_A1_T1()
        => ExecutionTestFromFile("S15.10.6_A1_T1");

    [Fact(DisplayName = "S15.10.6_A1_T2.js")]
    public Task S15_10_6_A1_T2()
        => ExecutionTestFromFile("S15.10.6_A1_T2");

    [Fact(DisplayName = "no-regexp-matcher.js")]
    public Task no_regexp_matcher()
        => ExecutionTestFromFile("no-regexp-matcher");

}
