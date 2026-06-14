using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.object_.dstr;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.object_.dstr") { }

    [Fact(DisplayName = "object-rest-proxy-get-not-called-on-dontenum-keys")]
    public Task object_rest_proxy_get_not_called_on_dontenum_keys()
        => ExecutionTest("object-rest-proxy-get-not-called-on-dontenum-keys");

    [Fact(DisplayName = "object-rest-proxy-gopd-not-called-on-excluded-keys")]
    public Task object_rest_proxy_gopd_not_called_on_excluded_keys()
        => ExecutionTest("object-rest-proxy-gopd-not-called-on-excluded-keys");

    [Fact(DisplayName = "object-rest-proxy-ownkeys-returned-keys-order")]
    public Task object_rest_proxy_ownkeys_returned_keys_order()
        => ExecutionTest("object-rest-proxy-ownkeys-returned-keys-order");
}
