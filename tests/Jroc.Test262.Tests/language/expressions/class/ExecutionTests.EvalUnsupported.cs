using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.class_;

public partial class ExecutionTests
{
    [Fact(DisplayName = "private-getter-brand-check-multiple-evaluations-of-class-eval-indirect.js", Skip = "Blocked: eval is not supported yet.")]
    public Task skipped_eval_private_getter_brand_check_multiple_evaluations_of_class_eval_indirect() => ExecutionTest("private-getter-brand-check-multiple-evaluations-of-class-eval-indirect");

    [Fact(DisplayName = "private-getter-brand-check-multiple-evaluations-of-class-realm.js", Skip = "Blocked: eval is not supported yet.")]
    public Task skipped_eval_private_getter_brand_check_multiple_evaluations_of_class_realm() => ExecutionTest("private-getter-brand-check-multiple-evaluations-of-class-realm");

    [Fact(DisplayName = "private-method-brand-check-multiple-evaluations-of-class-eval-indirect.js", Skip = "Blocked: eval is not supported yet.")]
    public Task skipped_eval_private_method_brand_check_multiple_evaluations_of_class_eval_indirect() => ExecutionTest("private-method-brand-check-multiple-evaluations-of-class-eval-indirect");

}
