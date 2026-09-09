using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.all;

public class CatalogFailureBatchExecutionTests : DiskExecutionTestsBase
{
    public CatalogFailureBatchExecutionTests() : base("built_ins.Promise.all") { }

    [Fact(DisplayName = "S25.4.4.1_A2.2_T1.js")]
    public Task S25_4_4_1_A2_2_T1() => ExecutionTestFromFile("S25.4.4.1_A2.2_T1");

    [Fact(DisplayName = "S25.4.4.1_A2.3_T1.js")]
    public Task S25_4_4_1_A2_3_T1() => ExecutionTestFromFile("S25.4.4.1_A2.3_T1");

    [Fact(DisplayName = "S25.4.4.1_A2.3_T2.js")]
    public Task S25_4_4_1_A2_3_T2() => ExecutionTestFromFile("S25.4.4.1_A2.3_T2");

    [Fact(DisplayName = "S25.4.4.1_A2.3_T3.js")]
    public Task S25_4_4_1_A2_3_T3() => ExecutionTestFromFile("S25.4.4.1_A2.3_T3");

    [Fact(DisplayName = "S25.4.4.1_A3.1_T1.js")]
    public Task S25_4_4_1_A3_1_T1() => ExecutionTestFromFile("S25.4.4.1_A3.1_T1");

    [Fact(DisplayName = "S25.4.4.1_A3.1_T2.js")]
    public Task S25_4_4_1_A3_1_T2() => ExecutionTestFromFile("S25.4.4.1_A3.1_T2");

    [Fact(DisplayName = "S25.4.4.1_A3.1_T3.js")]
    public Task S25_4_4_1_A3_1_T3() => ExecutionTestFromFile("S25.4.4.1_A3.1_T3");

    [Fact(DisplayName = "S25.4.4.1_A5.1_T1.js")]
    public Task S25_4_4_1_A5_1_T1() => ExecutionTestFromFile("S25.4.4.1_A5.1_T1");

    [Fact(DisplayName = "S25.4.4.1_A7.1_T1.js")]
    public Task S25_4_4_1_A7_1_T1() => ExecutionTestFromFile("S25.4.4.1_A7.1_T1");

    [Fact(DisplayName = "S25.4.4.1_A7.2_T1.js")]
    public Task S25_4_4_1_A7_2_T1() => ExecutionTestFromFile("S25.4.4.1_A7.2_T1");

    [Fact(DisplayName = "S25.4.4.1_A8.1_T1.js")]
    public Task S25_4_4_1_A8_1_T1() => ExecutionTestFromFile("S25.4.4.1_A8.1_T1");

    [Fact(DisplayName = "S25.4.4.1_A8.2_T1.js")]
    public Task S25_4_4_1_A8_2_T1() => ExecutionTestFromFile("S25.4.4.1_A8.2_T1");

    [Fact(DisplayName = "S25.4.4.1_A8.2_T2.js")]
    public Task S25_4_4_1_A8_2_T2() => ExecutionTestFromFile("S25.4.4.1_A8.2_T2");

    [Fact(DisplayName = "capability-resolve-throws-reject.js")]
    public Task capability_resolve_throws_reject() => ExecutionTestFromFile("capability-resolve-throws-reject");

    [Fact(DisplayName = "does-not-invoke-array-setters.js")]
    public Task does_not_invoke_array_setters() => ExecutionTestFromFile("does-not-invoke-array-setters");

    [Fact(DisplayName = "invoke-resolve-error-reject.js")]
    public Task invoke_resolve_error_reject() => ExecutionTestFromFile("invoke-resolve-error-reject");

    [Fact(DisplayName = "invoke-resolve-get-error-reject.js")]
    public Task invoke_resolve_get_error_reject() => ExecutionTestFromFile("invoke-resolve-get-error-reject");

    [Fact(DisplayName = "invoke-resolve-get-error.js")]
    public Task invoke_resolve_get_error() => ExecutionTestFromFile("invoke-resolve-get-error");

    [Fact(DisplayName = "invoke-resolve-on-promises-every-iteration-of-custom.js")]
    public Task invoke_resolve_on_promises_every_iteration_of_custom() => ExecutionTestFromFile("invoke-resolve-on-promises-every-iteration-of-custom");

    [Fact(DisplayName = "invoke-resolve-on-promises-every-iteration-of-promise.js")]
    public Task invoke_resolve_on_promises_every_iteration_of_promise() => ExecutionTestFromFile("invoke-resolve-on-promises-every-iteration-of-promise");

    [Fact(DisplayName = "invoke-resolve-on-values-every-iteration-of-promise.js")]
    public Task invoke_resolve_on_values_every_iteration_of_promise() => ExecutionTestFromFile("invoke-resolve-on-values-every-iteration-of-promise");

    [Fact(DisplayName = "invoke-then-error-reject.js")]
    public Task invoke_then_error_reject() => ExecutionTestFromFile("invoke-then-error-reject");

    [Fact(DisplayName = "invoke-then-get-error-reject.js")]
    public Task invoke_then_get_error_reject() => ExecutionTestFromFile("invoke-then-get-error-reject");

    [Fact(DisplayName = "iter-arg-is-false-reject.js")]
    public Task iter_arg_is_false_reject() => ExecutionTestFromFile("iter-arg-is-false-reject");

    [Fact(DisplayName = "iter-arg-is-null-reject.js")]
    public Task iter_arg_is_null_reject() => ExecutionTestFromFile("iter-arg-is-null-reject");

    [Fact(DisplayName = "iter-arg-is-number-reject.js")]
    public Task iter_arg_is_number_reject() => ExecutionTestFromFile("iter-arg-is-number-reject");

    [Fact(DisplayName = "iter-arg-is-string-resolve.js")]
    public Task iter_arg_is_string_resolve() => ExecutionTestFromFile("iter-arg-is-string-resolve");

    [Fact(DisplayName = "iter-arg-is-symbol-reject.js")]
    public Task iter_arg_is_symbol_reject() => ExecutionTestFromFile("iter-arg-is-symbol-reject");

    [Fact(DisplayName = "iter-arg-is-true-reject.js")]
    public Task iter_arg_is_true_reject() => ExecutionTestFromFile("iter-arg-is-true-reject");

    [Fact(DisplayName = "iter-arg-is-undefined-reject.js")]
    public Task iter_arg_is_undefined_reject() => ExecutionTestFromFile("iter-arg-is-undefined-reject");

    [Fact(DisplayName = "iter-assigned-false-reject.js")]
    public Task iter_assigned_false_reject() => ExecutionTestFromFile("iter-assigned-false-reject");

    [Fact(DisplayName = "iter-assigned-null-reject.js")]
    public Task iter_assigned_null_reject() => ExecutionTestFromFile("iter-assigned-null-reject");

    [Fact(DisplayName = "iter-assigned-number-reject.js")]
    public Task iter_assigned_number_reject() => ExecutionTestFromFile("iter-assigned-number-reject");

    [Fact(DisplayName = "iter-assigned-string-reject.js")]
    public Task iter_assigned_string_reject() => ExecutionTestFromFile("iter-assigned-string-reject");

    [Fact(DisplayName = "iter-assigned-symbol-reject.js")]
    public Task iter_assigned_symbol_reject() => ExecutionTestFromFile("iter-assigned-symbol-reject");

    [Fact(DisplayName = "iter-assigned-true-reject.js")]
    public Task iter_assigned_true_reject() => ExecutionTestFromFile("iter-assigned-true-reject");

    [Fact(DisplayName = "iter-assigned-undefined-reject.js")]
    public Task iter_assigned_undefined_reject() => ExecutionTestFromFile("iter-assigned-undefined-reject");

    [Fact(DisplayName = "iter-next-val-err-reject.js")]
    public Task iter_next_val_err_reject() => ExecutionTestFromFile("iter-next-val-err-reject");

    [Fact(DisplayName = "iter-returns-false-reject.js")]
    public Task iter_returns_false_reject() => ExecutionTestFromFile("iter-returns-false-reject");

    [Fact(DisplayName = "iter-returns-null-reject.js")]
    public Task iter_returns_null_reject() => ExecutionTestFromFile("iter-returns-null-reject");

    [Fact(DisplayName = "iter-returns-number-reject.js")]
    public Task iter_returns_number_reject() => ExecutionTestFromFile("iter-returns-number-reject");

    [Fact(DisplayName = "iter-returns-string-reject.js")]
    public Task iter_returns_string_reject() => ExecutionTestFromFile("iter-returns-string-reject");

    [Fact(DisplayName = "iter-returns-symbol-reject.js")]
    public Task iter_returns_symbol_reject() => ExecutionTestFromFile("iter-returns-symbol-reject");

    [Fact(DisplayName = "iter-returns-true-reject.js")]
    public Task iter_returns_true_reject() => ExecutionTestFromFile("iter-returns-true-reject");

    [Fact(DisplayName = "iter-returns-undefined-reject.js")]
    public Task iter_returns_undefined_reject() => ExecutionTestFromFile("iter-returns-undefined-reject");

    [Fact(DisplayName = "iter-step-err-reject.js")]
    public Task iter_step_err_reject() => ExecutionTestFromFile("iter-step-err-reject");

    [Fact(DisplayName = "reject-deferred.js")]
    public Task reject_deferred() => ExecutionTestFromFile("reject-deferred");

    [Fact(DisplayName = "reject-ignored-deferred.js")]
    public Task reject_ignored_deferred() => ExecutionTestFromFile("reject-ignored-deferred");

    [Fact(DisplayName = "reject-ignored-immed.js")]
    public Task reject_ignored_immed() => ExecutionTestFromFile("reject-ignored-immed");

    [Fact(DisplayName = "reject-immed.js")]
    public Task reject_immed() => ExecutionTestFromFile("reject-immed");

    [Fact(DisplayName = "resolve-ignores-late-rejection-deferred.js")]
    public Task resolve_ignores_late_rejection_deferred() => ExecutionTestFromFile("resolve-ignores-late-rejection-deferred");

    [Fact(DisplayName = "resolve-ignores-late-rejection.js")]
    public Task resolve_ignores_late_rejection() => ExecutionTestFromFile("resolve-ignores-late-rejection");

    [Fact(DisplayName = "resolve-non-callable.js")]
    public Task resolve_non_callable() => ExecutionTestFromFile("resolve-non-callable");

    [Fact(DisplayName = "resolve-non-thenable.js")]
    public Task resolve_non_thenable() => ExecutionTestFromFile("resolve-non-thenable");

    [Fact(DisplayName = "resolve-not-callable-reject-with-typeerror.js")]
    public Task resolve_not_callable_reject_with_typeerror() => ExecutionTestFromFile("resolve-not-callable-reject-with-typeerror");

    [Fact(DisplayName = "resolve-poisoned-then.js")]
    public Task resolve_poisoned_then() => ExecutionTestFromFile("resolve-poisoned-then");

    [Fact(DisplayName = "resolve-thenable.js")]
    public Task resolve_thenable() => ExecutionTestFromFile("resolve-thenable");
}
