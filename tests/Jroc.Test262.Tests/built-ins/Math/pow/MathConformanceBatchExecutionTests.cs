using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Math.pow;

public class MathConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public MathConformanceBatchExecutionTests() : base("built_ins.Math.pow") { }

    [Fact(DisplayName = "applying-the-exp-operator_A10.js")]
    public Task applying_the_exp_operator_A10() => ExecutionTestFromFile("applying-the-exp-operator_A10");

    [Fact(DisplayName = "applying-the-exp-operator_A11.js")]
    public Task applying_the_exp_operator_A11() => ExecutionTestFromFile("applying-the-exp-operator_A11");

    [Fact(DisplayName = "applying-the-exp-operator_A12.js")]
    public Task applying_the_exp_operator_A12() => ExecutionTestFromFile("applying-the-exp-operator_A12");

    [Fact(DisplayName = "applying-the-exp-operator_A13.js")]
    public Task applying_the_exp_operator_A13() => ExecutionTestFromFile("applying-the-exp-operator_A13");

    [Fact(DisplayName = "applying-the-exp-operator_A14.js")]
    public Task applying_the_exp_operator_A14() => ExecutionTestFromFile("applying-the-exp-operator_A14");

    [Fact(DisplayName = "applying-the-exp-operator_A15.js")]
    public Task applying_the_exp_operator_A15() => ExecutionTestFromFile("applying-the-exp-operator_A15");

    [Fact(DisplayName = "applying-the-exp-operator_A16.js")]
    public Task applying_the_exp_operator_A16() => ExecutionTestFromFile("applying-the-exp-operator_A16");

    [Fact(DisplayName = "applying-the-exp-operator_A17.js")]
    public Task applying_the_exp_operator_A17() => ExecutionTestFromFile("applying-the-exp-operator_A17");

    [Fact(DisplayName = "applying-the-exp-operator_A18.js")]
    public Task applying_the_exp_operator_A18() => ExecutionTestFromFile("applying-the-exp-operator_A18");

    [Fact(DisplayName = "applying-the-exp-operator_A19.js")]
    public Task applying_the_exp_operator_A19() => ExecutionTestFromFile("applying-the-exp-operator_A19");

    [Fact(DisplayName = "applying-the-exp-operator_A2.js")]
    public Task applying_the_exp_operator_A2() => ExecutionTestFromFile("applying-the-exp-operator_A2");

    [Fact(DisplayName = "applying-the-exp-operator_A20.js")]
    public Task applying_the_exp_operator_A20() => ExecutionTestFromFile("applying-the-exp-operator_A20");

    [Fact(DisplayName = "applying-the-exp-operator_A21.js")]
    public Task applying_the_exp_operator_A21() => ExecutionTestFromFile("applying-the-exp-operator_A21");

    [Fact(DisplayName = "applying-the-exp-operator_A22.js")]
    public Task applying_the_exp_operator_A22() => ExecutionTestFromFile("applying-the-exp-operator_A22");

    [Fact(DisplayName = "applying-the-exp-operator_A23.js")]
    public Task applying_the_exp_operator_A23() => ExecutionTestFromFile("applying-the-exp-operator_A23");

    [Fact(DisplayName = "applying-the-exp-operator_A3.js")]
    public Task applying_the_exp_operator_A3() => ExecutionTestFromFile("applying-the-exp-operator_A3");

    [Fact(DisplayName = "applying-the-exp-operator_A4.js")]
    public Task applying_the_exp_operator_A4() => ExecutionTestFromFile("applying-the-exp-operator_A4");

    [Fact(DisplayName = "applying-the-exp-operator_A5.js")]
    public Task applying_the_exp_operator_A5() => ExecutionTestFromFile("applying-the-exp-operator_A5");

    [Fact(DisplayName = "applying-the-exp-operator_A6.js")]
    public Task applying_the_exp_operator_A6() => ExecutionTestFromFile("applying-the-exp-operator_A6");

    [Fact(DisplayName = "applying-the-exp-operator_A9.js")]
    public Task applying_the_exp_operator_A9() => ExecutionTestFromFile("applying-the-exp-operator_A9");

    [Fact(DisplayName = "int32_min-exponent.js")]
    public Task int32_min_exponent() => ExecutionTestFromFile("int32_min-exponent");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

}
