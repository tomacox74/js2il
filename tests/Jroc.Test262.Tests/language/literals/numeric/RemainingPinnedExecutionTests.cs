namespace Jroc.Test262.Tests.language.literals.numeric;

public class RemainingPinnedExecutionTests : DiskExecutionTestsBase
{
    public RemainingPinnedExecutionTests() : base("language.literals.numeric") { }

    [Fact(DisplayName = "7.8.3-3gs.js", Skip = "Requires intrinsic eval execution, which JROC does not support.")]
    public Task _7_8_3_3gs()
        => ExecutionTest("7.8.3-3gs");

}
