namespace Jroc.Test262.Tests.built_ins.RegExp.lookBehind;

public sealed class RegExpRemainingExecutionTests : InMemoryExecutionTestsBase
{
    public RegExpRemainingExecutionTests() : base("built_ins.RegExp.lookBehind") { }

    [Fact(DisplayName = "back-references-to-captures.js")]
    public Task back_references_to_captures()
        => ExecutionTest("back-references-to-captures");

    [Fact(DisplayName = "misc.js")]
    public Task misc()
        => ExecutionTest("misc");

    [Fact(DisplayName = "mutual-recursive.js")]
    public Task mutual_recursive()
        => ExecutionTest("mutual-recursive");
}
