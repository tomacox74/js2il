namespace Jroc.Test262.Tests.language.expressions.import_meta.syntax;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.import_meta.syntax") { }

    [Fact(DisplayName = "escape-sequence-import.js")]
    public Task escape_sequence_import()
        => CompilationFailureTest("escape-sequence-import", "Failed to parse JavaScript");

    [Fact(DisplayName = "escape-sequence-meta.js")]
    public Task escape_sequence_meta()
        => CompilationFailureTest("escape-sequence-meta", "Failed to parse JavaScript");

    [Fact(DisplayName = "goal-async-function-params-or-body.js")]
    public Task goal_async_function_params_or_body()
        => ExecutionTest("goal-async-function-params-or-body");

    [Fact(DisplayName = "goal-async-generator-params-or-body.js")]
    public Task goal_async_generator_params_or_body()
        => ExecutionTest("goal-async-generator-params-or-body");

    [Fact(DisplayName = "goal-function-params-or-body.js")]
    public Task goal_function_params_or_body()
        => ExecutionTest("goal-function-params-or-body");

    [Fact(DisplayName = "goal-generator-params-or-body.js")]
    public Task goal_generator_params_or_body()
        => ExecutionTest("goal-generator-params-or-body");

    [Fact(DisplayName = "goal-module-nested-function.js")]
    public Task goal_module_nested_function()
        => ExecutionTest("goal-module-nested-function");

    [Fact(DisplayName = "goal-module.js")]
    public Task goal_module()
        => ExecutionTest("goal-module");

    [Fact(DisplayName = "goal-script.js")]
    public Task goal_script()
        => CompilationFailureTest("goal-script", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-assignment-target-array-destructuring-expr.js")]
    public Task invalid_assignment_target_array_destructuring_expr()
        => CompilationFailureTest("invalid-assignment-target-array-destructuring-expr", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-assignment-target-array-rest-destructuring-expr.js")]
    public Task invalid_assignment_target_array_rest_destructuring_expr()
        => CompilationFailureTest("invalid-assignment-target-array-rest-destructuring-expr", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-assignment-target-assignment-expr.js")]
    public Task invalid_assignment_target_assignment_expr()
        => CompilationFailureTest("invalid-assignment-target-assignment-expr", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-assignment-target-for-await-of-loop.js")]
    public Task invalid_assignment_target_for_await_of_loop()
        => CompilationFailureTest("invalid-assignment-target-for-await-of-loop", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-assignment-target-for-in-loop.js")]
    public Task invalid_assignment_target_for_in_loop()
        => CompilationFailureTest("invalid-assignment-target-for-in-loop", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-assignment-target-for-of-loop.js")]
    public Task invalid_assignment_target_for_of_loop()
        => CompilationFailureTest("invalid-assignment-target-for-of-loop", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-assignment-target-object-destructuring-expr.js")]
    public Task invalid_assignment_target_object_destructuring_expr()
        => CompilationFailureTest("invalid-assignment-target-object-destructuring-expr", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-assignment-target-object-rest-destructuring-expr.js")]
    public Task invalid_assignment_target_object_rest_destructuring_expr()
        => CompilationFailureTest("invalid-assignment-target-object-rest-destructuring-expr", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-assignment-target-update-expr.js")]
    public Task invalid_assignment_target_update_expr()
        => CompilationFailureTest("invalid-assignment-target-update-expr", "Failed to parse JavaScript");
}
