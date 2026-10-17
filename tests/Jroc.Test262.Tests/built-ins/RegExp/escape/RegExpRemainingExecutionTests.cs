namespace Jroc.Test262.Tests.built_ins.RegExp.escape;

public sealed class RegExpRemainingExecutionTests : InMemoryExecutionTestsBase
{
    public RegExpRemainingExecutionTests() : base("built_ins.RegExp.escape") { }

    [Fact(DisplayName = "cross-realm.js")]
    public Task cross_realm()
        => ExecutionTest("cross-realm");
}
