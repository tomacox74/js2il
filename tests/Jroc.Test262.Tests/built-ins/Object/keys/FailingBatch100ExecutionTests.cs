using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.keys;

public class FailingBatch100ExecutionTests : DiskExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Object.keys") { }

    [Fact(DisplayName = "proxy-keys")]
    public Task proxy_keys() => ExecutionTestFromFile("proxy-keys");

    [Fact(DisplayName = "proxy-non-enumerable-prop-invariant-1")]
    public Task proxy_non_enumerable_prop_invariant_1() => ExecutionTestFromFile("proxy-non-enumerable-prop-invariant-1");

    [Fact(DisplayName = "proxy-non-enumerable-prop-invariant-2")]
    public Task proxy_non_enumerable_prop_invariant_2() => ExecutionTestFromFile("proxy-non-enumerable-prop-invariant-2");

}
