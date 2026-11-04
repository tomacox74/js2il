namespace Jroc.Test262.Tests.language.literals.regexp.named_groups;

public class RemainingPinnedExecutionTests : DiskExecutionTestsBase
{
    public RemainingPinnedExecutionTests() : base("language.literals.regexp.named_groups") { }

    [Fact(DisplayName = "forward-reference.js")]
    public Task forward_reference()
        => ExecutionTest("forward-reference");

    [Fact(DisplayName = "invalid-lone-surrogate-groupname.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task invalid_lone_surrogate_groupname()
        => ExecutionTest("invalid-lone-surrogate-groupname");

}
