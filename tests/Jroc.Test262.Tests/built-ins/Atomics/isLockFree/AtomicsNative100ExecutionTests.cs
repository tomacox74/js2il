using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Atomics.isLockFree;

public class AtomicsNative100ExecutionTests : InMemoryExecutionTestsBase
{
    public AtomicsNative100ExecutionTests() : base("built_ins.Atomics.isLockFree") { }

    [Fact(DisplayName = "corner-cases.js")]
    public Task ported_corner_cases() => ExecutionTestFromFile("corner-cases");

    [Fact(DisplayName = "descriptor.js")]
    public Task ported_descriptor() => ExecutionTestFromFile("descriptor");

    [Fact(DisplayName = "expected-return-value.js")]
    public Task ported_expected_return_value() => ExecutionTestFromFile("expected-return-value");

    [Fact(DisplayName = "length.js")]
    public Task ported_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task ported_name() => ExecutionTestFromFile("name");

}
