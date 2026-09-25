using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Atomics.notify.bigint;

public sealed class AtomicsAgent100ExecutionTests : ExecutionTestsBase
{
    public AtomicsAgent100ExecutionTests() : base("built_ins.Atomics.notify.bigint") { }

    [Fact(DisplayName = "notify-all-on-loc.js")]
    public Task ported_notify_all_on_loc() => ExecutionTest("notify-all-on-loc");

}
