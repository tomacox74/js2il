using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Atomics.notify;

public class Port400ExecutionTests : InMemoryExecutionTestsBase
{
    public Port400ExecutionTests() : base("built_ins.Atomics.notify") { }

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port400_not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "null-bufferdata-throws.js")]
    public Task Port400_null_bufferdata_throws()
        => ExecutionTestFromFile("null-bufferdata-throws");
}
