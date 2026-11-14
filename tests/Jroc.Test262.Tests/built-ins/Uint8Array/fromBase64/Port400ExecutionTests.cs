using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Uint8Array.fromBase64;

public class Port400ExecutionTests : InMemoryExecutionTestsBase
{
    public Port400ExecutionTests() : base("built_ins.Uint8Array.fromBase64") { }

    [Fact(DisplayName = "descriptor.js")]
    public Task Port400_descriptor()
        => ExecutionTestFromFile("descriptor");

    [Fact(DisplayName = "ignores-receiver.js")]
    public Task Port400_ignores_receiver()
        => ExecutionTestFromFile("ignores-receiver");

    [Fact(DisplayName = "length.js")]
    public Task Port400_length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task Port400_name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "nonconstructor.js")]
    public Task Port400_nonconstructor()
        => ExecutionTestFromFile("nonconstructor");
}
