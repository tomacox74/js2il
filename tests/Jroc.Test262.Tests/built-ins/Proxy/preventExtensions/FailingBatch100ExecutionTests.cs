using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.preventExtensions;

public class FailingBatch100ExecutionTests : DiskExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Proxy.preventExtensions") { }

    [Fact(DisplayName = "return-false")]
    public Task return_false() => ExecutionTestFromFile("return-false");

}
