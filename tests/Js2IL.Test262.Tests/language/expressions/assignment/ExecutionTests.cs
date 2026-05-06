using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.assignment;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.assignment") { }

[Fact(DisplayName = "8.12.5-3-b_1")]
    public Task _8_12_5_3_b_1()
        => ExecutionTest("8.12.5-3-b_1");

[Fact(DisplayName = "8.12.5-3-b_2")]
    public Task _8_12_5_3_b_2()
        => ExecutionTest("8.12.5-3-b_2");

[Fact(DisplayName = "8.12.5-5-b_1")]
    public Task _8_12_5_5_b_1()
        => ExecutionTest("8.12.5-5-b_1");

[Fact(DisplayName = "8.14.4-8-b_1")]
    public Task _8_14_4_8_b_1()
        => ExecutionTest("8.14.4-8-b_1");

[Fact(DisplayName = "11.13.1-1-6-s")]
    public Task _11_13_1_1_6_s()
        => ExecutionTest("11.13.1-1-6-s");

[Fact(DisplayName = "11.13.1-1-s")]
    public Task _11_13_1_1_s()
        => ExecutionTest("11.13.1-1-s");

[Fact(DisplayName = "11.13.1-2-s")]
    public Task _11_13_1_2_s()
        => ExecutionTest("11.13.1-2-s");

[Fact(DisplayName = "11.13.1-3-s")]
    public Task _11_13_1_3_s()
        => ExecutionTest("11.13.1-3-s");

[Fact(DisplayName = "8.12.5-3-b_1")]
    public Task _8_12_5_3_b_1()
        => ExecutionTest("8.12.5-3-b_1");

[Fact(DisplayName = "8.12.5-3-b_2")]
    public Task _8_12_5_3_b_2()
        => ExecutionTest("8.12.5-3-b_2");

[Fact(DisplayName = "8.12.5-5-b_1")]
    public Task _8_12_5_5_b_1()
        => ExecutionTest("8.12.5-5-b_1");

[Fact(DisplayName = "8.14.4-8-b_1")]
    public Task _8_14_4_8_b_1()
        => ExecutionTest("8.14.4-8-b_1");

[Fact(DisplayName = "11.13.1-1-6-s")]
    public Task _11_13_1_1_6_s()
        => ExecutionTest("11.13.1-1-6-s");

[Fact(DisplayName = "11.13.1-1-s")]
    public Task _11_13_1_1_s()
        => ExecutionTest("11.13.1-1-s");

[Fact(DisplayName = "11.13.1-2-s")]
    public Task _11_13_1_2_s()
        => ExecutionTest("11.13.1-2-s");

[Fact(DisplayName = "11.13.1-3-s")]
    public Task _11_13_1_3_s()
        => ExecutionTest("11.13.1-3-s");

[Fact(DisplayName = "11.13.1-4-1", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task _11_13_1_4_1()
        => ExecutionTest("11.13.1-4-1");

[Fact(DisplayName = "11.13.1-4-14-s", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task _11_13_1_4_14_s()
        => ExecutionTest("11.13.1-4-14-s");
}
