using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.isArray;

public class ArrayConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public ArrayConformanceBatchExecutionTests() : base("built_ins.Array.isArray") { }

    [Fact(DisplayName = "proxy-revoked.js")]
    public Task proxy_revoked() => ExecutionTestFromFile("proxy-revoked");

    [Fact(DisplayName = "proxy.js")]
    public Task proxy() => ExecutionTestFromFile("proxy");

}
