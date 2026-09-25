using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.race;

public class CombinatorFailureExecutionTests : InMemoryExecutionTestsBase
{
    public CombinatorFailureExecutionTests() : base("built_ins.Promise.race") { }

    [Fact(DisplayName = "S25.4.4.3_A2.2_T3.js")]
    public Task S25_4_4_3_A2_2_T3() => ExecutionTestFromFile("S25.4.4.3_A2.2_T3");
    [Fact(DisplayName = "S25.4.4.3_A3.1_T1.js")]
    public Task S25_4_4_3_A3_1_T1() => ExecutionTestFromFile("S25.4.4.3_A3.1_T1");
    [Fact(DisplayName = "S25.4.4.3_A3.1_T2.js")]
    public Task S25_4_4_3_A3_1_T2() => ExecutionTestFromFile("S25.4.4.3_A3.1_T2");
    [Fact(DisplayName = "S25.4.4.3_A4.1_T1.js")]
    public Task S25_4_4_3_A4_1_T1() => ExecutionTestFromFile("S25.4.4.3_A4.1_T1");
    [Fact(DisplayName = "S25.4.4.3_A4.1_T2.js")]
    public Task S25_4_4_3_A4_1_T2() => ExecutionTestFromFile("S25.4.4.3_A4.1_T2");
    [Fact(DisplayName = "capability-executor-called-twice.js")]
    public Task capability_executor_called_twice() => ExecutionTestFromFile("capability-executor-called-twice");
    [Fact(DisplayName = "capability-executor-not-callable.js")]
    public Task capability_executor_not_callable() => ExecutionTestFromFile("capability-executor-not-callable");
    [Fact(DisplayName = "ctx-ctor-throws.js")]
    public Task ctx_ctor_throws() => ExecutionTestFromFile("ctx-ctor-throws");
    [Fact(DisplayName = "ctx-ctor.js")]
    public Task ctx_ctor() => ExecutionTestFromFile("ctx-ctor");
    [Fact(DisplayName = "ctx-non-ctor.js")]
    public Task ctx_non_ctor() => ExecutionTestFromFile("ctx-non-ctor");
    [Fact(DisplayName = "ctx-non-object.js")]
    public Task ctx_non_object() => ExecutionTestFromFile("ctx-non-object");
    [Fact(DisplayName = "invoke-resolve-error-close.js")]
    public Task invoke_resolve_error_close() => ExecutionTestFromFile("invoke-resolve-error-close");
    [Fact(DisplayName = "invoke-resolve-error-reject.js")]
    public Task invoke_resolve_error_reject() => ExecutionTestFromFile("invoke-resolve-error-reject");
    [Fact(DisplayName = "invoke-resolve-get-error-reject.js")]
    public Task invoke_resolve_get_error_reject() => ExecutionTestFromFile("invoke-resolve-get-error-reject");
    [Fact(DisplayName = "invoke-resolve-get-error.js")]
    public Task invoke_resolve_get_error() => ExecutionTestFromFile("invoke-resolve-get-error");
    [Fact(DisplayName = "invoke-resolve-get-once-multiple-calls.js")]
    public Task invoke_resolve_get_once_multiple_calls() => ExecutionTestFromFile("invoke-resolve-get-once-multiple-calls");
    [Fact(DisplayName = "invoke-resolve-get-once-no-calls.js")]
    public Task invoke_resolve_get_once_no_calls() => ExecutionTestFromFile("invoke-resolve-get-once-no-calls");
    [Fact(DisplayName = "invoke-resolve-on-promises-every-iteration-of-custom.js")]
    public Task invoke_resolve_on_promises_every_iteration_of_custom() => ExecutionTestFromFile("invoke-resolve-on-promises-every-iteration-of-custom");
    [Fact(DisplayName = "invoke-resolve-on-promises-every-iteration-of-promise.js")]
    public Task invoke_resolve_on_promises_every_iteration_of_promise() => ExecutionTestFromFile("invoke-resolve-on-promises-every-iteration-of-promise");
    [Fact(DisplayName = "invoke-resolve-on-values-every-iteration-of-promise.js")]
    public Task invoke_resolve_on_values_every_iteration_of_promise() => ExecutionTestFromFile("invoke-resolve-on-values-every-iteration-of-promise");
    [Fact(DisplayName = "invoke-resolve-return.js")]
    public Task invoke_resolve_return() => ExecutionTestFromFile("invoke-resolve-return");
    [Fact(DisplayName = "invoke-resolve.js")]
    public Task invoke_resolve() => ExecutionTestFromFile("invoke-resolve");
    [Fact(DisplayName = "invoke-then-error-close.js")]
    public Task invoke_then_error_close() => ExecutionTestFromFile("invoke-then-error-close");
    [Fact(DisplayName = "invoke-then-error-reject.js")]
    public Task invoke_then_error_reject() => ExecutionTestFromFile("invoke-then-error-reject");
    [Fact(DisplayName = "invoke-then-get-error-close.js")]
    public Task invoke_then_get_error_close() => ExecutionTestFromFile("invoke-then-get-error-close");
    [Fact(DisplayName = "invoke-then-get-error-reject.js")]
    public Task invoke_then_get_error_reject() => ExecutionTestFromFile("invoke-then-get-error-reject");
    [Fact(DisplayName = "invoke-then.js")]
    public Task invoke_then() => ExecutionTestFromFile("invoke-then");
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
}
