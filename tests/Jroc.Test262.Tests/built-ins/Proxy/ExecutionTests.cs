using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Proxy") { }

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "create-handler-is-revoked-proxy")]
    public Task create_handler_is_revoked_proxy()
        => ExecutionTestFromFile("create-handler-is-revoked-proxy");

    [Fact(DisplayName = "create-target-is-revoked-function-proxy")]
    public Task create_target_is_revoked_function_proxy()
        => ExecutionTestFromFile("create-target-is-revoked-function-proxy");

    [Fact(DisplayName = "create-target-is-revoked-proxy")]
    public Task create_target_is_revoked_proxy()
        => ExecutionTestFromFile("create-target-is-revoked-proxy");

    [Fact(DisplayName = "function-prototype")]
    public Task function_prototype()
        => ExecutionTestFromFile("function-prototype");

    [Fact(DisplayName = "property-order")]
    public Task property_order()
        => ExecutionTestFromFile("property-order");

    [Fact(DisplayName = "proxy-newtarget")]
    public Task proxy_newtarget()
        => ExecutionTestFromFile("proxy-newtarget");

    [Fact(DisplayName = "proxy-no-prototype")]
    public Task proxy_no_prototype()
        => ExecutionTestFromFile("proxy-no-prototype");
}
