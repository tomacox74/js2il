using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.tagged_template;

public sealed class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.tagged-template") { }

    [Fact(DisplayName = "tco-call")]
    public Task tco_call()
        => ExecutionTest("tco-call");

    [Fact(DisplayName = "tco-member")]
    public Task tco_member()
        => ExecutionTest("tco-member");
}
