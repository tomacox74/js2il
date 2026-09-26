using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Atomics.waitAsync;

public sealed class AtomicsAgent100ExecutionTests : ExecutionTestsBase
{
    public AtomicsAgent100ExecutionTests() : base("built_ins.Atomics.waitAsync") { }

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

}
