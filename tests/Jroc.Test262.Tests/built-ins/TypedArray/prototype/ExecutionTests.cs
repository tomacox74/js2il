using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("TypedArray.prototype") { }

    [Fact(DisplayName = "constructor")]
    public Task constructor() => ExecutionTestFromFile("constructor");
}
