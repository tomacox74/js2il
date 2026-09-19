using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.revocable;

public class Test262BatchPort20260919Round3ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919Round3ExecutionTests() : base("built_ins.Proxy.revocable") { }

    [Fact(DisplayName = "handler-is-revoked-proxy")]
    public Task handler_is_revoked_proxy()
        => ExecutionTestFromFile("handler-is-revoked-proxy");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "proxy")]
    public Task proxy()
        => ExecutionTestFromFile("proxy");

    [Fact(DisplayName = "revocation-function-extensible")]
    public Task revocation_function_extensible()
        => ExecutionTestFromFile("revocation-function-extensible");

    [Fact(DisplayName = "revocation-function-length")]
    public Task revocation_function_length()
        => ExecutionTestFromFile("revocation-function-length");

    [Fact(DisplayName = "revocation-function-not-a-constructor")]
    public Task revocation_function_not_a_constructor()
        => ExecutionTestFromFile("revocation-function-not-a-constructor");

    [Fact(DisplayName = "revocation-function-property-order")]
    public Task revocation_function_property_order()
        => ExecutionTestFromFile("revocation-function-property-order");

    [Fact(DisplayName = "revocation-function-prototype")]
    public Task revocation_function_prototype()
        => ExecutionTestFromFile("revocation-function-prototype");

    [Fact(DisplayName = "revoke-consecutive-call-returns-undefined")]
    public Task revoke_consecutive_call_returns_undefined()
        => ExecutionTestFromFile("revoke-consecutive-call-returns-undefined");

    [Fact(DisplayName = "revoke-returns-undefined")]
    public Task revoke_returns_undefined()
        => ExecutionTestFromFile("revoke-returns-undefined");

    [Fact(DisplayName = "revoke")]
    public Task revoke()
        => ExecutionTestFromFile("revoke");

    [Fact(DisplayName = "target-is-revoked-function-proxy")]
    public Task target_is_revoked_function_proxy()
        => ExecutionTestFromFile("target-is-revoked-function-proxy");

    [Fact(DisplayName = "target-is-revoked-proxy")]
    public Task target_is_revoked_proxy()
        => ExecutionTestFromFile("target-is-revoked-proxy");

}
