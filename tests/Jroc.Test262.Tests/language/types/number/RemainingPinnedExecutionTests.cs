namespace Jroc.Test262.Tests.language.types.number;

public class RemainingPinnedExecutionTests : DiskExecutionTestsBase
{
    public RemainingPinnedExecutionTests() : base("language.types.number") { }

    [Fact(DisplayName = "S8.5_A5.js")]
    public Task S8_5_A5()
        => ExecutionTest("S8.5_A5");

}
