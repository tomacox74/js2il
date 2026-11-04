namespace Jroc.Test262.Tests.language.types.@object;

public class RemainingPinnedExecutionTests : DiskExecutionTestsBase
{
    public RemainingPinnedExecutionTests() : base("language.types.object") { }

    [Fact(DisplayName = "S8.6.2_A5_T3.js")]
    public Task S8_6_2_A5_T3()
        => ExecutionTest("S8.6.2_A5_T3");

}
