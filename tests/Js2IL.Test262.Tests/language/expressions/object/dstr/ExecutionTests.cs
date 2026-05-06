using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.object_.dstr;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.object_.dstr") { }

    [Fact(DisplayName = "object-rest-proxy-get-not-called-on-dontenum-keys", Skip = "Object rest destructuring Proxy enumeration semantics are incomplete.")]
    public Task object_rest_proxy_get_not_called_on_dontenum_keys()
        => ExecutionTest("object-rest-proxy-get-not-called-on-dontenum-keys");
}
