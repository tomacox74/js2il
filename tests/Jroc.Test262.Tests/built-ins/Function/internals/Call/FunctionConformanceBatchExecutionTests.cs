using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Function.internals.Call;

public class FunctionConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public FunctionConformanceBatchExecutionTests() : base("built_ins.Function.internals.Call") { }

    [Fact(DisplayName = "class-ctor.js")]
    public Task class_ctor() => ExecutionTestFromFile("class-ctor");

}
