namespace Jroc.Test262.Tests.language.function_code;

public class FunctionCodeCompletionExecutionTests : DiskExecutionTestsBase
{
    public FunctionCodeCompletionExecutionTests() : base("language.function_code") { }

    [Fact(DisplayName = "10.4.3-1-17-s.js", Skip = "eval is not supported.")]
    public Task _10_4_3_1_17_s() => ExecutionTest("10.4.3-1-17-s");

    [Fact(DisplayName = "10.4.3-1-17gs.js", Skip = "eval is not supported.")]
    public Task _10_4_3_1_17gs() => ExecutionTest("10.4.3-1-17gs");

    [Fact(DisplayName = "10.4.3-1-18gs.js", Skip = "eval is not supported.")]
    public Task _10_4_3_1_18gs() => ExecutionTest("10.4.3-1-18gs");

    [Fact(DisplayName = "10.4.3-1-19-s.js", Skip = "eval is not supported.")]
    public Task _10_4_3_1_19_s() => ExecutionTest("10.4.3-1-19-s");

    [Fact(DisplayName = "10.4.3-1-19gs.js", Skip = "eval is not supported.")]
    public Task _10_4_3_1_19gs() => ExecutionTest("10.4.3-1-19gs");

    [Fact(DisplayName = "10.4.3-1-20-s.js", Skip = "eval is not supported.")]
    public Task _10_4_3_1_20_s() => ExecutionTest("10.4.3-1-20-s");

    [Fact(DisplayName = "10.4.3-1-20gs.js", Skip = "eval is not supported.")]
    public Task _10_4_3_1_20gs() => ExecutionTest("10.4.3-1-20gs");

    [Fact(DisplayName = "10.4.3-1-63-s.js", Skip = "eval is not supported.")]
    public Task _10_4_3_1_63_s() => ExecutionTest("10.4.3-1-63-s");

    [Fact(DisplayName = "10.4.3-1-63gs.js", Skip = "eval is not supported.")]
    public Task _10_4_3_1_63gs() => ExecutionTest("10.4.3-1-63gs");

    [Fact(DisplayName = "10.4.3-1-64gs.js")]
    public Task _10_4_3_1_64gs() => ExecutionTest("10.4.3-1-64gs");

    [Fact(DisplayName = "10.4.3-1-65gs.js")]
    public Task _10_4_3_1_65gs() => ExecutionTest("10.4.3-1-65gs");

    [Fact(DisplayName = "10.4.3-1-82-s.js", Skip = "eval is not supported.")]
    public Task _10_4_3_1_82_s() => ExecutionTest("10.4.3-1-82-s");

    [Fact(DisplayName = "10.4.3-1-82gs.js", Skip = "eval is not supported.")]
    public Task _10_4_3_1_82gs() => ExecutionTest("10.4.3-1-82gs");

    [Fact(DisplayName = "10.4.3-1-83gs.js")]
    public Task _10_4_3_1_83gs() => ExecutionTest("10.4.3-1-83gs");

    [Fact(DisplayName = "10.4.3-1-84gs.js")]
    public Task _10_4_3_1_84gs() => ExecutionTest("10.4.3-1-84gs");

    [Fact(DisplayName = "S10.2.1_A2.js")]
    public Task S10_2_1_A2() => ExecutionTest("S10.2.1_A2");

    [Fact(DisplayName = "S10.2.1_A3.js")]
    public Task S10_2_1_A3() => ExecutionTest("S10.2.1_A3");

    [Fact(DisplayName = "S10.2.1_A4_T1.js")]
    public Task S10_2_1_A4_T1() => ExecutionTest("S10.2.1_A4_T1");

    [Fact(DisplayName = "S10.2.1_A4_T2.js")]
    public Task S10_2_1_A4_T2() => ExecutionTest("S10.2.1_A4_T2");

    [Fact(DisplayName = "eval-param-env-with-computed-key.js", Skip = "eval is not supported.")]
    public Task eval_param_env_with_computed_key() => ExecutionTest("eval-param-env-with-computed-key");

    [Fact(DisplayName = "eval-param-env-with-prop-initializer.js", Skip = "eval is not supported.")]
    public Task eval_param_env_with_prop_initializer() => ExecutionTest("eval-param-env-with-prop-initializer");
}
