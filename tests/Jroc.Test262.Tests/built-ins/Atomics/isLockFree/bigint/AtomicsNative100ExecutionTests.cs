using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Atomics.isLockFree.bigint;

public class AtomicsNative100ExecutionTests : InMemoryExecutionTestsBase
{
    public AtomicsNative100ExecutionTests() : base("built_ins.Atomics.isLockFree.bigint") { }

    [Fact(DisplayName = "expected-return-value.js")]
    public Task ported_expected_return_value() => ExecutionTestFromFile("expected-return-value");

}
