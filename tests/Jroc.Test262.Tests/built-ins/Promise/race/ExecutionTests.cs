using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.race;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    [Fact(DisplayName = "S25.4.4.3_A2.2_T1.js")] public Task S25_4_4_3_A2_2_T1() => ExecutionTestFromFile("S25.4.4.3_A2.2_T1");
    [Fact(DisplayName = "S25.4.4.3_A2.2_T2.js")] public Task S25_4_4_3_A2_2_T2() => ExecutionTestFromFile("S25.4.4.3_A2.2_T2");
    [Fact(DisplayName = "S25.4.4.3_A5.1_T1.js")] public Task S25_4_4_3_A5_1_T1() => ExecutionTestFromFile("S25.4.4.3_A5.1_T1");
    [Fact(DisplayName = "S25.4.4.3_A6.1_T1.js")] public Task S25_4_4_3_A6_1_T1() => ExecutionTestFromFile("S25.4.4.3_A6.1_T1");
    [Fact(DisplayName = "S25.4.4.3_A6.2_T1.js")] public Task S25_4_4_3_A6_2_T1() => ExecutionTestFromFile("S25.4.4.3_A6.2_T1");
    [Fact(DisplayName = "S25.4.4.3_A7.1_T1.js")] public Task S25_4_4_3_A7_1_T1() => ExecutionTestFromFile("S25.4.4.3_A7.1_T1");
    [Fact(DisplayName = "S25.4.4.3_A7.1_T2.js")] public Task S25_4_4_3_A7_1_T2() => ExecutionTestFromFile("S25.4.4.3_A7.1_T2");
    [Fact(DisplayName = "S25.4.4.3_A7.1_T3.js")] public Task S25_4_4_3_A7_1_T3() => ExecutionTestFromFile("S25.4.4.3_A7.1_T3");
    [Fact(DisplayName = "S25.4.4.3_A7.2_T1.js")] public Task S25_4_4_3_A7_2_T1() => ExecutionTestFromFile("S25.4.4.3_A7.2_T1");
    [Fact(DisplayName = "S25.4.4.3_A7.3_T1.js")] public Task S25_4_4_3_A7_3_T1() => ExecutionTestFromFile("S25.4.4.3_A7.3_T1");
    [Fact(DisplayName = "S25.4.4.3_A7.3_T2.js")] public Task S25_4_4_3_A7_3_T2() => ExecutionTestFromFile("S25.4.4.3_A7.3_T2");
    [Fact(DisplayName = "iter-arg-is-false-reject.js")] public Task iter_arg_is_false_reject() => ExecutionTestFromFile("iter-arg-is-false-reject");
    [Fact(DisplayName = "iter-arg-is-null-reject.js")] public Task iter_arg_is_null_reject() => ExecutionTestFromFile("iter-arg-is-null-reject");
    [Fact(DisplayName = "iter-arg-is-number-reject.js")] public Task iter_arg_is_number_reject() => ExecutionTestFromFile("iter-arg-is-number-reject");
    [Fact(DisplayName = "iter-arg-is-string-resolve.js")] public Task iter_arg_is_string_resolve() => ExecutionTestFromFile("iter-arg-is-string-resolve");
    [Fact(DisplayName = "iter-arg-is-symbol-reject.js")] public Task iter_arg_is_symbol_reject() => ExecutionTestFromFile("iter-arg-is-symbol-reject");
    [Fact(DisplayName = "iter-arg-is-true-reject.js")] public Task iter_arg_is_true_reject() => ExecutionTestFromFile("iter-arg-is-true-reject");
    [Fact(DisplayName = "iter-arg-is-undefined-reject.js")] public Task iter_arg_is_undefined_reject() => ExecutionTestFromFile("iter-arg-is-undefined-reject");
    [Fact(DisplayName = "iter-step-err-reject.js")] public Task iter_step_err_reject() => ExecutionTestFromFile("iter-step-err-reject");
    [Fact(DisplayName = "reject-deferred.js")] public Task reject_deferred() => ExecutionTestFromFile("reject-deferred");
    [Fact(DisplayName = "reject-from-same-thenable.js")] public Task reject_from_same_thenable() => ExecutionTestFromFile("reject-from-same-thenable");
    [Fact(DisplayName = "reject-ignored-deferred.js")] public Task reject_ignored_deferred() => ExecutionTestFromFile("reject-ignored-deferred");
    [Fact(DisplayName = "reject-ignored-immed.js")] public Task reject_ignored_immed() => ExecutionTestFromFile("reject-ignored-immed");
    [Fact(DisplayName = "reject-immed.js")] public Task reject_immed() => ExecutionTestFromFile("reject-immed");
    [Fact(DisplayName = "resolve-from-same-thenable.js")] public Task resolve_from_same_thenable() => ExecutionTestFromFile("resolve-from-same-thenable");
    [Fact(DisplayName = "resolve-ignores-late-rejection-deferred.js")] public Task resolve_ignores_late_rejection_deferred() => ExecutionTestFromFile("resolve-ignores-late-rejection-deferred");
    [Fact(DisplayName = "resolve-ignores-late-rejection.js")] public Task resolve_ignores_late_rejection() => ExecutionTestFromFile("resolve-ignores-late-rejection");
    [Fact(DisplayName = "resolve-non-callable.js")] public Task resolve_non_callable() => ExecutionTestFromFile("resolve-non-callable");
    [Fact(DisplayName = "resolve-non-obj.js")] public Task resolve_non_obj() => ExecutionTestFromFile("resolve-non-obj");
    [Fact(DisplayName = "resolve-non-thenable.js")] public Task resolve_non_thenable() => ExecutionTestFromFile("resolve-non-thenable");
    [Fact(DisplayName = "resolve-poisoned-then.js")] public Task resolve_poisoned_then() => ExecutionTestFromFile("resolve-poisoned-then");
    [Fact(DisplayName = "resolve-prms-cstm-then.js")] public Task resolve_prms_cstm_then() => ExecutionTestFromFile("resolve-prms-cstm-then");
    [Fact(DisplayName = "resolve-self.js")] public Task resolve_self() => ExecutionTestFromFile("resolve-self");
    [Fact(DisplayName = "resolve-thenable.js")] public Task resolve_thenable() => ExecutionTestFromFile("resolve-thenable");
    [Fact(DisplayName = "resolve-throws-iterator-return-is-not-callable.js")] public Task resolve_throws_iterator_return_is_not_callable() => ExecutionTestFromFile("resolve-throws-iterator-return-is-not-callable");
    [Fact(DisplayName = "resolve-throws-iterator-return-null-or-undefined.js")] public Task resolve_throws_iterator_return_null_or_undefined() => ExecutionTestFromFile("resolve-throws-iterator-return-null-or-undefined");
    [Fact(DisplayName = "resolved-sequence-extra-ticks.js")] public Task resolved_sequence_extra_ticks() => ExecutionTestFromFile("resolved-sequence-extra-ticks");
    [Fact(DisplayName = "resolved-sequence-mixed.js")] public Task resolved_sequence_mixed() => ExecutionTestFromFile("resolved-sequence-mixed");
    [Fact(DisplayName = "resolved-sequence-with-rejections.js")] public Task resolved_sequence_with_rejections() => ExecutionTestFromFile("resolved-sequence-with-rejections");
    [Fact(DisplayName = "resolved-sequence.js")] public Task resolved_sequence() => ExecutionTestFromFile("resolved-sequence");
    [Fact(DisplayName = "resolved-then-catch-finally.js")] public Task resolved_then_catch_finally() => ExecutionTestFromFile("resolved-then-catch-finally");
    [Fact(DisplayName = "same-reject-function.js")] public Task same_reject_function() => ExecutionTestFromFile("same-reject-function");
    [Fact(DisplayName = "same-resolve-function.js")] public Task same_resolve_function() => ExecutionTestFromFile("same-resolve-function");

    public ExecutionTests() : base("built_ins.Promise.race") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

}
