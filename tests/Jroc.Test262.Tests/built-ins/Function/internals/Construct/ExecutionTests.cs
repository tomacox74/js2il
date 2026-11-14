using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Function.internals.Construct;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Function.internals.Construct") { }

    [Fact(DisplayName = "base-ctor-revoked-proxy-realm.js")]
    public Task Port200_base_ctor_revoked_proxy_realm()
        => ExecutionTestFromFile("base-ctor-revoked-proxy-realm");

    [Fact(DisplayName = "base-ctor-revoked-proxy.js")]
    public Task Port200_base_ctor_revoked_proxy()
        => ExecutionTestFromFile("base-ctor-revoked-proxy");
}
