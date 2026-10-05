using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.sort;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Array.prototype.sort") { }

    [Fact(DisplayName = "S15.4.4.11_A1.2_T1")]
    public Task S15_4_4_11_A1_2_T1()
        => ExecutionTestFromFile("S15.4.4.11_A1.2_T1");

    [Fact(DisplayName = "S15.4.4.11_A1.2_T2")]
    public Task S15_4_4_11_A1_2_T2()
        => ExecutionTestFromFile("S15.4.4.11_A1.2_T2");

    [Fact(DisplayName = "S15.4.4.11_A1.3_T1")]
    public Task S15_4_4_11_A1_3_T1()
        => ExecutionTestFromFile("S15.4.4.11_A1.3_T1");

    [Fact(DisplayName = "S15.4.4.11_A1.4_T1")]
    public Task S15_4_4_11_A1_4_T1()
        => ExecutionTestFromFile("S15.4.4.11_A1.4_T1");

    [Fact(DisplayName = "S15.4.4.11_A1.5_T1")]
    public Task S15_4_4_11_A1_5_T1()
        => ExecutionTestFromFile("S15.4.4.11_A1.5_T1");

    [Fact(DisplayName = "S15.4.4.11_A2.1_T1")]
    public Task S15_4_4_11_A2_1_T1()
        => ExecutionTestFromFile("S15.4.4.11_A2.1_T1");

    [Fact(DisplayName = "S15.4.4.11_A2.1_T2")]
    public Task S15_4_4_11_A2_1_T2()
        => ExecutionTestFromFile("S15.4.4.11_A2.1_T2");

    [Fact(DisplayName = "S15.4.4.11_A2.1_T3")]
    public Task S15_4_4_11_A2_1_T3()
        => ExecutionTestFromFile("S15.4.4.11_A2.1_T3");

    [Fact(DisplayName = "S15.4.4.11_A2.2_T1")]
    public Task S15_4_4_11_A2_2_T1()
        => ExecutionTestFromFile("S15.4.4.11_A2.2_T1");

    [Fact(DisplayName = "S15.4.4.11_A2.2_T2")]
    public Task S15_4_4_11_A2_2_T2()
        => ExecutionTestFromFile("S15.4.4.11_A2.2_T2");

    [Fact(DisplayName = "S15.4.4.11_A5_T1")]
    public Task S15_4_4_11_A5_T1()
        => ExecutionTestFromFile("S15.4.4.11_A5_T1");

    [Fact(DisplayName = "S15.4.4.11_A8")]
    public Task S15_4_4_11_A8()
        => ExecutionTestFromFile("S15.4.4.11_A8");

    [Fact(DisplayName = "bug_596_1")]
    public Task bug_596_1()
        => ExecutionTestFromFile("bug_596_1");

    [Fact(DisplayName = "bug_596_2")]
    public Task bug_596_2()
        => ExecutionTestFromFile("bug_596_2");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "precise-comparefn-throws")]
    public Task precise_comparefn_throws()
        => ExecutionTestFromFile("precise-comparefn-throws");

    [Fact(DisplayName = "precise-getter-decreases-length")]
    public Task precise_getter_decreases_length()
        => ExecutionTestFromFile("precise-getter-decreases-length");

    [Fact(DisplayName = "precise-getter-deletes-predecessor")]
    public Task precise_getter_deletes_predecessor()
        => ExecutionTestFromFile("precise-getter-deletes-predecessor");

    [Fact(DisplayName = "precise-getter-deletes-successor")]
    public Task precise_getter_deletes_successor()
        => ExecutionTestFromFile("precise-getter-deletes-successor");

    [Fact(DisplayName = "precise-getter-increases-length")]
    public Task precise_getter_increases_length()
        => ExecutionTestFromFile("precise-getter-increases-length");

    [Fact(DisplayName = "precise-getter-pops-elements")]
    public Task precise_getter_pops_elements()
        => ExecutionTestFromFile("precise-getter-pops-elements");

    [Fact(DisplayName = "precise-getter-sets-predecessor")]
    public Task precise_getter_sets_predecessor()
        => ExecutionTestFromFile("precise-getter-sets-predecessor");

    [Fact(DisplayName = "precise-getter-sets-successor")]
    public Task precise_getter_sets_successor()
        => ExecutionTestFromFile("precise-getter-sets-successor");

    [Fact(DisplayName = "precise-prototype-accessors")]
    public Task precise_prototype_accessors()
        => ExecutionTestFromFile("precise-prototype-accessors");

    [Fact(DisplayName = "precise-prototype-element")]
    public Task precise_prototype_element()
        => ExecutionTestFromFile("precise-prototype-element");

    [Fact(DisplayName = "precise-setter-deletes-predecessor")]
    public Task precise_setter_deletes_predecessor()
        => ExecutionTestFromFile("precise-setter-deletes-predecessor");

    [Fact(DisplayName = "precise-setter-deletes-successor")]
    public Task precise_setter_deletes_successor()
        => ExecutionTestFromFile("precise-setter-deletes-successor");

    [Fact(DisplayName = "precise-setter-increases-length")]
    public Task precise_setter_increases_length()
        => ExecutionTestFromFile("precise-setter-increases-length");

    [Fact(DisplayName = "precise-setter-sets-predecessor")]
    public Task precise_setter_sets_predecessor()
        => ExecutionTestFromFile("precise-setter-sets-predecessor");

    [Fact(DisplayName = "precise-setter-sets-successor")]
    public Task precise_setter_sets_successor()
        => ExecutionTestFromFile("precise-setter-sets-successor");

    [Fact(DisplayName = "stability-11-elements")]
    public Task stability_11_elements()
        => ExecutionTestFromFile("stability-11-elements");

    [Fact(DisplayName = "stability-5-elements")]
    public Task stability_5_elements()
        => ExecutionTestFromFile("stability-5-elements");

}
