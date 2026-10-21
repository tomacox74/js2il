namespace Jroc.Test262.Tests.language.statements.variable;

public sealed class VariableWithCompletionExecutionTests : DiskExecutionTestsBase
{
    public VariableWithCompletionExecutionTests() : base("language.statements.variable") { }

    [Fact(DisplayName = "12.2.1-10-s.js", Skip = "eval is not supported.")]
    public Task test_12_2_1_10_s()
        => ExecutionTest("12.2.1-10-s");

    [Fact(DisplayName = "12.2.1-11.js", Skip = "eval is not supported.")]
    public Task test_12_2_1_11()
        => ExecutionTest("12.2.1-11");

    [Fact(DisplayName = "12.2.1-18-s.js", Skip = "eval is not supported.")]
    public Task test_12_2_1_18_s()
        => ExecutionTest("12.2.1-18-s");

    [Fact(DisplayName = "12.2.1-19-s.js", Skip = "eval is not supported.")]
    public Task test_12_2_1_19_s()
        => ExecutionTest("12.2.1-19-s");

    [Fact(DisplayName = "12.2.1-1gs.js")]
    public Task test_12_2_1_1gs()
        => CompilationFailureTest("12.2.1-1gs", "Failed to parse JavaScript");

    [Fact(DisplayName = "12.2.1-2-s.js", Skip = "eval is not supported.")]
    public Task test_12_2_1_2_s()
        => ExecutionTest("12.2.1-2-s");

    [Fact(DisplayName = "12.2.1-20-s.js", Skip = "eval is not supported.")]
    public Task test_12_2_1_20_s()
        => ExecutionTest("12.2.1-20-s");

    [Fact(DisplayName = "12.2.1-21-s.js", Skip = "eval is not supported.")]
    public Task test_12_2_1_21_s()
        => ExecutionTest("12.2.1-21-s");

    [Fact(DisplayName = "12.2.1-22-s.js", Skip = "eval is not supported.")]
    public Task test_12_2_1_22_s()
        => ExecutionTest("12.2.1-22-s");

    [Fact(DisplayName = "12.2.1-3-s.js", Skip = "eval is not supported.")]
    public Task test_12_2_1_3_s()
        => ExecutionTest("12.2.1-3-s");

    [Fact(DisplayName = "12.2.1-4-s.js", Skip = "eval is not supported.")]
    public Task test_12_2_1_4_s()
        => ExecutionTest("12.2.1-4-s");

    [Fact(DisplayName = "12.2.1-4gs.js")]
    public Task test_12_2_1_4gs()
        => CompilationFailureTest("12.2.1-4gs", "Failed to parse JavaScript");

    [Fact(DisplayName = "12.2.1-7-s.js", Skip = "eval is not supported.")]
    public Task test_12_2_1_7_s()
        => ExecutionTest("12.2.1-7-s");

    [Fact(DisplayName = "12.2.1-8-s.js", Skip = "eval is not supported.")]
    public Task test_12_2_1_8_s()
        => ExecutionTest("12.2.1-8-s");

    [Fact(DisplayName = "12.2.1-9-s.js", Skip = "eval is not supported.")]
    public Task test_12_2_1_9_s()
        => ExecutionTest("12.2.1-9-s");

    [Fact(DisplayName = "S12.2_A11.js")]
    public Task S12_2_A11()
        => ExecutionTest("S12.2_A11");

    [Fact(DisplayName = "S12.2_A5.js", Skip = "eval is not supported.")]
    public Task S12_2_A5()
        => ExecutionTest("S12.2_A5");

    [Fact(DisplayName = "S12.2_A8_T1.js")]
    public Task S12_2_A8_T1()
        => CompilationFailureTest("S12.2_A8_T1", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.2_A8_T2.js")]
    public Task S12_2_A8_T2()
        => CompilationFailureTest("S12.2_A8_T2", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.2_A8_T3.js")]
    public Task S12_2_A8_T3()
        => CompilationFailureTest("S12.2_A8_T3", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.2_A8_T4.js")]
    public Task S12_2_A8_T4()
        => CompilationFailureTest("S12.2_A8_T4", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.2_A8_T5.js")]
    public Task S12_2_A8_T5()
        => CompilationFailureTest("S12.2_A8_T5", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.2_A8_T6.js")]
    public Task S12_2_A8_T6()
        => CompilationFailureTest("S12.2_A8_T6", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.2_A8_T7.js")]
    public Task S12_2_A8_T7()
        => CompilationFailureTest("S12.2_A8_T7", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.2_A8_T8.js")]
    public Task S12_2_A8_T8()
        => CompilationFailureTest("S12.2_A8_T8", "Failed to parse JavaScript");

    [Fact(DisplayName = "arguments-fn-strict-list-final-init.js")]
    public Task arguments_fn_strict_list_final_init()
        => CompilationFailureTest("arguments-fn-strict-list-final-init", "Failed to parse JavaScript");

    [Fact(DisplayName = "arguments-fn-strict-list-final.js")]
    public Task arguments_fn_strict_list_final()
        => CompilationFailureTest("arguments-fn-strict-list-final", "Failed to parse JavaScript");

    [Fact(DisplayName = "arguments-fn-strict-list-first-init.js")]
    public Task arguments_fn_strict_list_first_init()
        => CompilationFailureTest("arguments-fn-strict-list-first-init", "Failed to parse JavaScript");

    [Fact(DisplayName = "arguments-fn-strict-list-first.js")]
    public Task arguments_fn_strict_list_first()
        => CompilationFailureTest("arguments-fn-strict-list-first", "Failed to parse JavaScript");

    [Fact(DisplayName = "arguments-fn-strict-list-middle-init.js")]
    public Task arguments_fn_strict_list_middle_init()
        => CompilationFailureTest("arguments-fn-strict-list-middle-init", "Failed to parse JavaScript");

    [Fact(DisplayName = "arguments-fn-strict-list-middle.js")]
    public Task arguments_fn_strict_list_middle()
        => CompilationFailureTest("arguments-fn-strict-list-middle", "Failed to parse JavaScript");

    [Fact(DisplayName = "arguments-fn-strict-list-repeated.js")]
    public Task arguments_fn_strict_list_repeated()
        => CompilationFailureTest("arguments-fn-strict-list-repeated", "Failed to parse JavaScript");

    [Fact(DisplayName = "arguments-fn-strict-single-init.js")]
    public Task arguments_fn_strict_single_init()
        => CompilationFailureTest("arguments-fn-strict-single-init", "Failed to parse JavaScript");

    [Fact(DisplayName = "arguments-fn-strict-single.js")]
    public Task arguments_fn_strict_single()
        => CompilationFailureTest("arguments-fn-strict-single", "Failed to parse JavaScript");

    [Fact(DisplayName = "arguments-strict-list-final-init.js")]
    public Task arguments_strict_list_final_init()
        => CompilationFailureTest("arguments-strict-list-final-init", "Failed to parse JavaScript");

    [Fact(DisplayName = "arguments-strict-list-final.js")]
    public Task arguments_strict_list_final()
        => CompilationFailureTest("arguments-strict-list-final", "Failed to parse JavaScript");

    [Fact(DisplayName = "arguments-strict-list-first-init.js")]
    public Task arguments_strict_list_first_init()
        => CompilationFailureTest("arguments-strict-list-first-init", "Failed to parse JavaScript");

    [Fact(DisplayName = "arguments-strict-list-first.js")]
    public Task arguments_strict_list_first()
        => CompilationFailureTest("arguments-strict-list-first", "Failed to parse JavaScript");

    [Fact(DisplayName = "arguments-strict-list-middle-init.js")]
    public Task arguments_strict_list_middle_init()
        => CompilationFailureTest("arguments-strict-list-middle-init", "Failed to parse JavaScript");

    [Fact(DisplayName = "arguments-strict-list-middle.js")]
    public Task arguments_strict_list_middle()
        => CompilationFailureTest("arguments-strict-list-middle", "Failed to parse JavaScript");

    [Fact(DisplayName = "arguments-strict-list-repeated.js")]
    public Task arguments_strict_list_repeated()
        => CompilationFailureTest("arguments-strict-list-repeated", "Failed to parse JavaScript");

    [Fact(DisplayName = "arguments-strict-single-init.js")]
    public Task arguments_strict_single_init()
        => CompilationFailureTest("arguments-strict-single-init", "Failed to parse JavaScript");

    [Fact(DisplayName = "arguments-strict-single.js")]
    public Task arguments_strict_single()
        => CompilationFailureTest("arguments-strict-single", "Failed to parse JavaScript");

    [Fact(DisplayName = "cptn-value.js", Skip = "eval is not supported.")]
    public Task cptn_value()
        => ExecutionTest("cptn-value");

    [Fact(DisplayName = "eval-strict-list-final-init.js")]
    public Task eval_strict_list_final_init()
        => CompilationFailureTest("eval-strict-list-final-init", "Failed to parse JavaScript");

    [Fact(DisplayName = "eval-strict-list-final.js")]
    public Task eval_strict_list_final()
        => CompilationFailureTest("eval-strict-list-final", "Failed to parse JavaScript");

    [Fact(DisplayName = "eval-strict-list-first-init.js")]
    public Task eval_strict_list_first_init()
        => CompilationFailureTest("eval-strict-list-first-init", "Failed to parse JavaScript");

    [Fact(DisplayName = "eval-strict-list-first.js")]
    public Task eval_strict_list_first()
        => CompilationFailureTest("eval-strict-list-first", "Failed to parse JavaScript");

    [Fact(DisplayName = "eval-strict-list-middle-init.js")]
    public Task eval_strict_list_middle_init()
        => CompilationFailureTest("eval-strict-list-middle-init", "Failed to parse JavaScript");

    [Fact(DisplayName = "eval-strict-list-middle.js")]
    public Task eval_strict_list_middle()
        => CompilationFailureTest("eval-strict-list-middle", "Failed to parse JavaScript");

    [Fact(DisplayName = "eval-strict-list-repeated.js")]
    public Task eval_strict_list_repeated()
        => CompilationFailureTest("eval-strict-list-repeated", "Failed to parse JavaScript");

    [Fact(DisplayName = "eval-strict-single-init.js")]
    public Task eval_strict_single_init()
        => CompilationFailureTest("eval-strict-single-init", "Failed to parse JavaScript");

    [Fact(DisplayName = "eval-strict-single.js")]
    public Task eval_strict_single()
        => CompilationFailureTest("eval-strict-single", "Failed to parse JavaScript");

    [Fact(DisplayName = "id-arguments-strict.js")]
    public Task id_arguments_strict()
        => CompilationFailureTest("id-arguments-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "id-eval-strict.js")]
    public Task id_eval_strict()
        => CompilationFailureTest("id-eval-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-await-binding-invalid.js")]
    public Task static_init_await_binding_invalid()
        => CompilationFailureTest("static-init-await-binding-invalid", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-await-binding-valid.js")]
    public Task static_init_await_binding_valid()
        => ExecutionTest("static-init-await-binding-valid");
}
