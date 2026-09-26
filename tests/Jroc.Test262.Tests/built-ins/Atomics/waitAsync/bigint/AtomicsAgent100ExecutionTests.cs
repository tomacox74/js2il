using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Atomics.waitAsync.bigint;

public sealed class AtomicsAgent100ExecutionTests : ExecutionTestsBase
{
    public AtomicsAgent100ExecutionTests() : base("built_ins.Atomics.waitAsync.bigint") { }

    [Fact(DisplayName = "false-for-timeout-agent.js")]
    public Task ported_false_for_timeout_agent() => ExecutionTest("false-for-timeout-agent");

    [Fact(DisplayName = "good-views.js")]
    public Task ported_good_views() => ExecutionTest("good-views");

    [Fact(DisplayName = "nan-for-timeout-agent.js")]
    public Task ported_nan_for_timeout_agent() => ExecutionTest("nan-for-timeout-agent");

    [Fact(DisplayName = "negative-timeout-agent.js")]
    public Task ported_negative_timeout_agent() => ExecutionTest("negative-timeout-agent");

    [Fact(DisplayName = "no-spurious-wakeup-no-operation.js")]
    public Task ported_no_spurious_wakeup_no_operation() => ExecutionTest("no-spurious-wakeup-no-operation");

    [Fact(DisplayName = "no-spurious-wakeup-on-add.js")]
    public Task ported_no_spurious_wakeup_on_add() => ExecutionTest("no-spurious-wakeup-on-add");

    [Fact(DisplayName = "no-spurious-wakeup-on-and.js")]
    public Task ported_no_spurious_wakeup_on_and() => ExecutionTest("no-spurious-wakeup-on-and");

    [Fact(DisplayName = "no-spurious-wakeup-on-compareExchange.js")]
    public Task ported_no_spurious_wakeup_on_compareExchange() => ExecutionTest("no-spurious-wakeup-on-compareExchange");

    [Fact(DisplayName = "no-spurious-wakeup-on-exchange.js")]
    public Task ported_no_spurious_wakeup_on_exchange() => ExecutionTest("no-spurious-wakeup-on-exchange");

    [Fact(DisplayName = "no-spurious-wakeup-on-or.js")]
    public Task ported_no_spurious_wakeup_on_or() => ExecutionTest("no-spurious-wakeup-on-or");

    [Fact(DisplayName = "no-spurious-wakeup-on-store.js")]
    public Task ported_no_spurious_wakeup_on_store() => ExecutionTest("no-spurious-wakeup-on-store");

    [Fact(DisplayName = "no-spurious-wakeup-on-sub.js")]
    public Task ported_no_spurious_wakeup_on_sub() => ExecutionTest("no-spurious-wakeup-on-sub");

    [Fact(DisplayName = "no-spurious-wakeup-on-xor.js")]
    public Task ported_no_spurious_wakeup_on_xor() => ExecutionTest("no-spurious-wakeup-on-xor");

    [Fact(DisplayName = "null-for-timeout-agent.js")]
    public Task ported_null_for_timeout_agent() => ExecutionTest("null-for-timeout-agent");

    [Fact(DisplayName = "object-for-timeout-agent.js")]
    public Task ported_object_for_timeout_agent() => ExecutionTest("object-for-timeout-agent");

    [Fact(DisplayName = "poisoned-object-for-timeout-throws-agent.js")]
    public Task ported_poisoned_object_for_timeout_throws_agent() => ExecutionTest("poisoned-object-for-timeout-throws-agent");

    [Fact(DisplayName = "symbol-for-index-throws-agent.js")]
    public Task ported_symbol_for_index_throws_agent() => ExecutionTest("symbol-for-index-throws-agent");

    [Fact(DisplayName = "symbol-for-timeout-throws-agent.js")]
    public Task ported_symbol_for_timeout_throws_agent() => ExecutionTest("symbol-for-timeout-throws-agent");

    [Fact(DisplayName = "symbol-for-value-throws-agent.js")]
    public Task ported_symbol_for_value_throws_agent() => ExecutionTest("symbol-for-value-throws-agent");

    [Fact(DisplayName = "true-for-timeout-agent.js")]
    public Task ported_true_for_timeout_agent() => ExecutionTest("true-for-timeout-agent");

    [Fact(DisplayName = "true-for-timeout.js")]
    public Task ported_true_for_timeout() => ExecutionTest("true-for-timeout");

    [Fact(DisplayName = "undefined-for-timeout-agent.js")]
    public Task ported_undefined_for_timeout_agent() => ExecutionTest("undefined-for-timeout-agent");

    [Fact(DisplayName = "undefined-index-defaults-to-zero-agent.js")]
    public Task ported_undefined_index_defaults_to_zero_agent() => ExecutionTest("undefined-index-defaults-to-zero-agent");

    [Fact(DisplayName = "value-not-equal-agent.js")]
    public Task ported_value_not_equal_agent() => ExecutionTest("value-not-equal-agent");

    [Fact(DisplayName = "waiterlist-block-indexedposition-wake.js")]
    public Task ported_waiterlist_block_indexedposition_wake() => ExecutionTest("waiterlist-block-indexedposition-wake");

    [Fact(DisplayName = "was-woken-before-timeout.js")]
    public Task ported_was_woken_before_timeout() => ExecutionTest("was-woken-before-timeout");

}
