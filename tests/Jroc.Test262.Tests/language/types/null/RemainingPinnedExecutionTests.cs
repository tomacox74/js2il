namespace Jroc.Test262.Tests.language.types.@null;

public class RemainingPinnedExecutionTests : DiskExecutionTestsBase
{
    public RemainingPinnedExecutionTests() : base("language.types.null") { }

    [Fact(DisplayName = "S8.2_A2.js")]
    public Task S8_2_A2()
        => CompilationFailureTest("S8.2_A2");

}
