using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.NativeErrors.ReferenceError;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.NativeErrors.ReferenceError") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");
}
