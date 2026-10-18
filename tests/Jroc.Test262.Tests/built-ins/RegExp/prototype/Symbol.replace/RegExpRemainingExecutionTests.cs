namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.Symbol_replace;

public sealed class RegExpRemainingExecutionTests : InMemoryExecutionTestsBase
{
    public RegExpRemainingExecutionTests() : base("built_ins.RegExp.prototype.Symbol_replace") { }

    [Fact(DisplayName = "coerce-global.js")]
    public Task coerce_global()
        => ExecutionTest("coerce-global");
}
