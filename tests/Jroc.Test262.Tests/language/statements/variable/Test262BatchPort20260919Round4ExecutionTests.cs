using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.statements.variable;

public class Test262BatchPort20260919Round4ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260919Round4ExecutionTests() : base("language.statements.variable") { }

    [Fact(DisplayName = "12.2.1-16-s")]
    public Task _12_2_1_16_s()
        => ExecutionTest("12.2.1-16-s");

    [Fact(DisplayName = "12.2.1-17-s")]
    public Task _12_2_1_17_s()
        => ExecutionTest("12.2.1-17-s");

    [Fact(DisplayName = "12.2.1-5-s")]
    public Task _12_2_1_5_s()
        => ExecutionTest("12.2.1-5-s");

    [Fact(DisplayName = "12.2.1-6-s")]
    public Task _12_2_1_6_s()
        => ExecutionTest("12.2.1-6-s");

    [Fact(DisplayName = "S12.2_A1")]
    public Task S12_2_A1()
        => ExecutionTest("S12.2_A1");

    [Fact(DisplayName = "S12.2_A10")]
    public Task S12_2_A10()
        => ExecutionTest("S12.2_A10");

    [Fact(DisplayName = "S12.2_A12")]
    public Task S12_2_A12()
        => ExecutionTest("S12.2_A12");

    [Fact(DisplayName = "S12.2_A6_T1")]
    public Task S12_2_A6_T1()
        => ExecutionTest("S12.2_A6_T1");

    [Fact(DisplayName = "S12.2_A6_T2")]
    public Task S12_2_A6_T2()
        => ExecutionTest("S12.2_A6_T2");

    [Fact(DisplayName = "S12.2_A7")]
    public Task S12_2_A7()
        => ExecutionTest("S12.2_A7");

    [Fact(DisplayName = "S12.2_A9")]
    public Task S12_2_A9()
        => ExecutionTest("S12.2_A9");

    [Fact(DisplayName = "S14_A1")]
    public Task S14_A1()
        => ExecutionTest("S14_A1");

    [Fact(DisplayName = "arguments-fn-non-strict")]
    public Task arguments_fn_non_strict()
        => ExecutionTest("arguments-fn-non-strict");

    [Fact(DisplayName = "fn-name-arrow")]
    public Task fn_name_arrow()
        => ExecutionTest("fn-name-arrow");

    [Fact(DisplayName = "fn-name-class")]
    public Task fn_name_class()
        => ExecutionTest("fn-name-class");

    [Fact(DisplayName = "fn-name-cover")]
    public Task fn_name_cover()
        => ExecutionTest("fn-name-cover");

    [Fact(DisplayName = "fn-name-fn")]
    public Task fn_name_fn()
        => ExecutionTest("fn-name-fn");

    [Fact(DisplayName = "fn-name-gen")]
    public Task fn_name_gen()
        => ExecutionTest("fn-name-gen");

}
