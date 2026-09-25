using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Atomics.add;

public class AtomicsNative100ExecutionTests : InMemoryExecutionTestsBase
{
    public AtomicsNative100ExecutionTests() : base("built_ins.Atomics.add") { }

    [Fact(DisplayName = "bad-range.js")]
    public Task ported_bad_range() => ExecutionTestFromFile("bad-range");

    [Fact(DisplayName = "descriptor.js")]
    public Task ported_descriptor() => ExecutionTestFromFile("descriptor");

    [Fact(DisplayName = "expected-return-value.js")]
    public Task ported_expected_return_value() => ExecutionTestFromFile("expected-return-value");

    [Fact(DisplayName = "good-views.js")]
    public Task ported_good_views() => ExecutionTestFromFile("good-views");

    [Fact(DisplayName = "length.js")]
    public Task ported_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task ported_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "non-shared-bufferdata.js")]
    public Task ported_non_shared_bufferdata() => ExecutionTestFromFile("non-shared-bufferdata");

}
