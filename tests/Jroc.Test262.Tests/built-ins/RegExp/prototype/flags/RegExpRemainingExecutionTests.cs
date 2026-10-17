namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.flags;

public sealed class RegExpRemainingExecutionTests : InMemoryExecutionTestsBase
{
    public RegExpRemainingExecutionTests() : base("built_ins.RegExp.prototype.flags") { }

    [Fact(DisplayName = "this-val-regexp.js")]
    public Task this_val_regexp()
        => ExecutionTest("this-val-regexp");
}
