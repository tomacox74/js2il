using Jroc.Test262.Tests.built_ins;


namespace Jroc.Test262.Tests.built_ins.Array.prototype.sort;


public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.sort") { }

    [Fact(DisplayName = "S15.4.4.11_A1.1_T1")]
    public Task S15_4_4_11_A1_1_T1()
        => ExecutionTestFromFile("S15.4.4.11_A1.1_T1");

    [Fact(DisplayName = "S15.4.4.11_A1.4_T2")]
    public Task S15_4_4_11_A1_4_T2()
        => ExecutionTestFromFile("S15.4.4.11_A1.4_T2");

    [Fact(DisplayName = "S15.4.4.11_A2.2_T3")]
    public Task S15_4_4_11_A2_2_T3()
        => ExecutionTestFromFile("S15.4.4.11_A2.2_T3");

    [Fact(DisplayName = "S15.4.4.11_A3_T1")]
    public Task S15_4_4_11_A3_T1()
        => ExecutionTestFromFile("S15.4.4.11_A3_T1");

    [Fact(DisplayName = "S15.4.4.11_A3_T2")]
    public Task S15_4_4_11_A3_T2()
        => ExecutionTestFromFile("S15.4.4.11_A3_T2");

    [Fact(DisplayName = "S15.4.4.11_A4_T3")]
    public Task S15_4_4_11_A4_T3()
        => ExecutionTestFromFile("S15.4.4.11_A4_T3");

    [Fact(DisplayName = "S15.4.4.11_A6_T2")]
    public Task S15_4_4_11_A6_T2()
        => ExecutionTestFromFile("S15.4.4.11_A6_T2");

    [Fact(DisplayName = "call-with-primitive")]
    public Task call_with_primitive()
        => ExecutionTestFromFile("call-with-primitive");

    [Fact(DisplayName = "comparefn-nonfunction-call-throws")]
    public Task comparefn_nonfunction_call_throws()
        => ExecutionTestFromFile("comparefn-nonfunction-call-throws");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "precise-getter-appends-elements")]
    public Task precise_getter_appends_elements()
        => ExecutionTestFromFile("precise-getter-appends-elements");

    [Fact(DisplayName = "precise-setter-appends-elements")]
    public Task precise_setter_appends_elements()
        => ExecutionTestFromFile("precise-setter-appends-elements");

    [Fact(DisplayName = "precise-setter-decreases-length")]
    public Task precise_setter_decreases_length()
        => ExecutionTestFromFile("precise-setter-decreases-length");

    [Fact(DisplayName = "precise-setter-pops-elements")]
    public Task precise_setter_pops_elements()
        => ExecutionTestFromFile("precise-setter-pops-elements");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "stability-2048-elements")]
    public Task stability_2048_elements()
        => ExecutionTestFromFile("stability-2048-elements");

    [Fact(DisplayName = "stability-513-elements")]
    public Task stability_513_elements()
        => ExecutionTestFromFile("stability-513-elements");
}
