using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Atomics.pause;

public class AtomicsNative100ExecutionTests : InMemoryExecutionTestsBase
{
    public AtomicsNative100ExecutionTests() : base("built_ins.Atomics.pause") { }

    [Fact(DisplayName = "length.js")]
    public Task ported_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task ported_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "returns-undefined.js")]
    public Task ported_returns_undefined() => ExecutionTestFromFile("returns-undefined");

}
