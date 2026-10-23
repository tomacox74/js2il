namespace Jroc.Test262.Tests.language.expressions.tagged_template;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.tagged_template") { }

    [Fact(DisplayName = "cache-differing-expressions-eval.js", Skip = "eval is not supported.")]
    public Task cache_differing_expressions_eval()
        => ExecutionTest("cache-differing-expressions-eval");

    [Fact(DisplayName = "cache-differing-expressions-new-function.js")]
    public Task cache_differing_expressions_new_function()
        => ExecutionTest("cache-differing-expressions-new-function");

    [Fact(DisplayName = "cache-eval-inner-function.js", Skip = "eval is not supported.")]
    public Task cache_eval_inner_function()
        => ExecutionTest("cache-eval-inner-function");

    [Fact(DisplayName = "cache-identical-source-eval.js", Skip = "eval is not supported.")]
    public Task cache_identical_source_eval()
        => ExecutionTest("cache-identical-source-eval");

    [Fact(DisplayName = "cache-identical-source-new-function.js")]
    public Task cache_identical_source_new_function()
        => ExecutionTest("cache-identical-source-new-function");

    [Fact(DisplayName = "cache-realm.js", Skip = "eval is not supported.")]
    public Task cache_realm()
        => ExecutionTest("cache-realm");

    [Fact(DisplayName = "call-expression-argument-list-evaluation.js")]
    public Task call_expression_argument_list_evaluation()
        => ExecutionTest("call-expression-argument-list-evaluation");

    [Fact(DisplayName = "chained-application.js")]
    public Task chained_application()
        => ExecutionTest("chained-application");

    [Fact(DisplayName = "invalid-escape-sequences.js")]
    public Task invalid_escape_sequences()
        => ExecutionTest("invalid-escape-sequences");

    [Fact(DisplayName = "member-expression-argument-list-evaluation.js")]
    public Task member_expression_argument_list_evaluation()
        => ExecutionTest("member-expression-argument-list-evaluation");

    [Fact(DisplayName = "member-expression-context.js")]
    public Task member_expression_context()
        => ExecutionTest("member-expression-context");

    [Fact(DisplayName = "template-object-frozen-non-strict.js")]
    public Task template_object_frozen_non_strict()
        => ExecutionTest("template-object-frozen-non-strict");

    [Fact(DisplayName = "template-object-frozen-strict.js")]
    public Task template_object_frozen_strict()
        => ExecutionTest("template-object-frozen-strict");

    [Fact(DisplayName = "template-object.js")]
    public Task template_object()
        => ExecutionTest("template-object");
}
