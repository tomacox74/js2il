using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.function;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.function") { }

    [Fact(DisplayName = "13.0-12-s")]
    public Task _13_0_12_s()
        => ExecutionTest("13.0-12-s");

    [Fact(DisplayName = "13.0-13-s", Skip = "Blocked: eval is not supported yet.")]
    public Task _13_0_13_s()
        => ExecutionTest("13.0-13-s");

    [Fact(DisplayName = "13.0-14-s", Skip = "Blocked: eval is not supported yet.")]
    public Task _13_0_14_s()
        => ExecutionTest("13.0-14-s");

    [Fact(DisplayName = "13.0-15-s", Skip = "Blocked: eval is not supported yet.")]
    public Task _13_0_15_s()
        => ExecutionTest("13.0-15-s");

    [Fact(DisplayName = "13.0-16-s", Skip = "Blocked: eval is not supported yet.")]
    public Task _13_0_16_s()
        => ExecutionTest("13.0-16-s");

    [Fact(DisplayName = "13.0-17-s", Skip = "Blocked: eval is not supported yet.")]
    public Task _13_0_17_s()
        => ExecutionTest("13.0-17-s");

    [Fact(DisplayName = "13.0-7-s", Skip = "Blocked: eval is not supported yet.")]
    public Task _13_0_7_s()
        => ExecutionTest("13.0-7-s");

    [Fact(DisplayName = "13.0-8-s", Skip = "Blocked: eval is not supported yet.")]
    public Task _13_0_8_s()
        => ExecutionTest("13.0-8-s");

    [Fact(DisplayName = "13.0_4-17gs")]
    public Task _13_0_4_17gs()
        => ExecutionTest("13.0_4-17gs");

    [Fact(DisplayName = "13.1-19-s", Skip = "Blocked: eval is not supported yet.")]
    public Task _13_1_19_s()
        => ExecutionTest("13.1-19-s");

    [Fact(DisplayName = "13.1-2-s", Skip = "Blocked: eval is not supported yet.")]
    public Task _13_1_2_s()
        => ExecutionTest("13.1-2-s");

    [Fact(DisplayName = "13.1-21-s", Skip = "Blocked: eval is not supported yet.")]
    public Task _13_1_21_s()
        => ExecutionTest("13.1-21-s");

    [Fact(DisplayName = "13.1-22-s", Skip = "Blocked: eval is not supported yet.")]
    public Task _13_1_22_s()
        => ExecutionTest("13.1-22-s");

    [Fact(DisplayName = "13.1-23-s", Skip = "Blocked: eval is not supported yet.")]
    public Task _13_1_23_s()
        => ExecutionTest("13.1-23-s");

    [Fact(DisplayName = "13.1-25-s", Skip = "Blocked: eval is not supported yet.")]
    public Task _13_1_25_s()
        => ExecutionTest("13.1-25-s");

    [Fact(DisplayName = "13.1-27-s", Skip = "Blocked: eval is not supported yet.")]
    public Task _13_1_27_s()
        => ExecutionTest("13.1-27-s");
    [Fact(DisplayName = "13.2-1-s")]
    public Task _13_2_1_s()
        => ExecutionTest("13.2-1-s");

    [Fact(DisplayName = "13.2-19-b-3gs")]
    public Task _13_2_19_b_3gs()
        => ExecutionTest("13.2-19-b-3gs");

    [Fact(DisplayName = "13.2-2-s")]
    public Task _13_2_2_s()
        => ExecutionTest("13.2-2-s");

    [Fact(DisplayName = "13.2-21-s")]
    public Task _13_2_21_s()
        => ExecutionTest("13.2-21-s");

    [Fact(DisplayName = "13.2-22-s")]
    public Task _13_2_22_s()
        => ExecutionTest("13.2-22-s");

    [Fact(DisplayName = "13.2-23-s")]
    public Task _13_2_23_s()
        => ExecutionTest("13.2-23-s");

    [Fact(DisplayName = "13.2-24-s")]
    public Task _13_2_24_s()
        => ExecutionTest("13.2-24-s");

    [Fact(DisplayName = "13.2-25-s")]
    public Task _13_2_25_s()
        => ExecutionTest("13.2-25-s");

    [Fact(DisplayName = "13.2-26-s")]
    public Task _13_2_26_s()
        => ExecutionTest("13.2-26-s");

    [Fact(DisplayName = "13.2-27-s")]
    public Task _13_2_27_s()
        => ExecutionTest("13.2-27-s");

}
