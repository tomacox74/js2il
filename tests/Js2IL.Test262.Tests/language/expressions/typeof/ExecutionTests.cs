using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.typeof_;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.typeof_") { }

    [Fact(DisplayName = "proxy")]
    public Task proxy()
        => ExecutionTest("proxy");
}
