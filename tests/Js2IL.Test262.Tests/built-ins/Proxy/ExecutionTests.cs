using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Proxy;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Proxy") { }

    [Fact(DisplayName = "constructor", Skip = "Tracked by #1093: JS2IL does not yet pass this advanced test262 scenario.")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "create-handler-is-revoked-proxy")]
    public Task create_handler_is_revoked_proxy()
        => ExecutionTestFromFile("create-handler-is-revoked-proxy");

    [Fact(DisplayName = "create-target-is-revoked-function-proxy", Skip = "Tracked by #1093: JS2IL does not yet pass this advanced test262 scenario.")]
    public Task create_target_is_revoked_function_proxy()
        => ExecutionTestFromFile("create-target-is-revoked-function-proxy");

    [Fact(DisplayName = "create-target-is-revoked-proxy", Skip = "Tracked by #1093: JS2IL does not yet pass this advanced test262 scenario.")]
    public Task create_target_is_revoked_proxy()
        => ExecutionTestFromFile("create-target-is-revoked-proxy");

    [Fact(DisplayName = "function-prototype", Skip = "Tracked by #1093: JS2IL does not yet pass this advanced test262 scenario.")]
    public Task function_prototype()
        => ExecutionTestFromFile("function-prototype");

    [Fact(DisplayName = "property-order", Skip = "Tracked by #1093: JS2IL does not yet pass this advanced test262 scenario.")]
    public Task property_order()
        => ExecutionTestFromFile("property-order");

    [Fact(DisplayName = "proxy-newtarget")]
    public Task proxy_newtarget()
        => ExecutionTestFromFile("proxy-newtarget");

    [Fact(DisplayName = "proxy-no-prototype", Skip = "Tracked by #1093: JS2IL does not yet pass this advanced test262 scenario.")]
    public Task proxy_no_prototype()
        => ExecutionTestFromFile("proxy-no-prototype");
}
