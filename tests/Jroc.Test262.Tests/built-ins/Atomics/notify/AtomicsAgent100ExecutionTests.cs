using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Atomics.notify;

public sealed class AtomicsAgent100ExecutionTests : ExecutionTestsBase
{
    public AtomicsAgent100ExecutionTests() : base("built_ins.Atomics.notify") { }

    [Fact(DisplayName = "count-defaults-to-infinity-missing.js")]
    public Task ported_count_defaults_to_infinity_missing() => ExecutionTest("count-defaults-to-infinity-missing");

    [Fact(DisplayName = "count-defaults-to-infinity-undefined.js")]
    public Task ported_count_defaults_to_infinity_undefined() => ExecutionTest("count-defaults-to-infinity-undefined");

    [Fact(DisplayName = "negative-count.js")]
    public Task ported_negative_count() => ExecutionTest("negative-count");

    [Fact(DisplayName = "notify-all-on-loc.js")]
    public Task ported_notify_all_on_loc() => ExecutionTest("notify-all-on-loc");

    [Fact(DisplayName = "notify-all.js")]
    public Task ported_notify_all() => ExecutionTest("notify-all");

    [Fact(DisplayName = "notify-in-order-one-time.js")]
    public Task ported_notify_in_order_one_time() => ExecutionTest("notify-in-order-one-time");

    [Fact(DisplayName = "notify-in-order.js")]
    public Task ported_notify_in_order() => ExecutionTest("notify-in-order");

    [Fact(DisplayName = "notify-nan.js")]
    public Task ported_notify_nan() => ExecutionTest("notify-nan");

    [Fact(DisplayName = "notify-one.js")]
    public Task ported_notify_one() => ExecutionTest("notify-one");

    [Fact(DisplayName = "notify-renotify-noop.js")]
    public Task ported_notify_renotify_noop() => ExecutionTest("notify-renotify-noop");

    [Fact(DisplayName = "notify-two.js")]
    public Task ported_notify_two() => ExecutionTest("notify-two");

    [Fact(DisplayName = "notify-with-no-agents-waiting.js")]
    public Task ported_notify_with_no_agents_waiting() => ExecutionTest("notify-with-no-agents-waiting");

    [Fact(DisplayName = "notify-with-no-matching-agents-waiting.js")]
    public Task ported_notify_with_no_matching_agents_waiting() => ExecutionTest("notify-with-no-matching-agents-waiting");

    [Fact(DisplayName = "notify-zero.js")]
    public Task ported_notify_zero() => ExecutionTest("notify-zero");

    [Fact(DisplayName = "undefined-index-defaults-to-zero.js")]
    public Task ported_undefined_index_defaults_to_zero() => ExecutionTest("undefined-index-defaults-to-zero");

}
