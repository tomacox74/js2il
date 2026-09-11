using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.revocable;

public class FailingBatch100ExecutionTests : DiskExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Proxy.revocable") { }

    [Fact(DisplayName = "builtin")]
    public Task builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "revocation-function-name")]
    public Task revocation_function_name() => ExecutionTestFromFile("revocation-function-name");

}
