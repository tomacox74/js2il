using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Atomics.wait;

public class Port400ExecutionTests : InMemoryExecutionTestsBase
{
    public Port400ExecutionTests() : base("built_ins.Atomics.wait") { }

    [Fact(DisplayName = "null-bufferdata-throws.js")]
    public Task Port400_null_bufferdata_throws()
        => ExecutionTestFromFile("null-bufferdata-throws");
}
