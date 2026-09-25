using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.NativeErrors.RangeError;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.NativeErrors.RangeError") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");
}
