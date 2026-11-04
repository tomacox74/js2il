namespace Jroc.Test262.Tests.language.types.boolean;

public class RemainingPinnedExecutionTests : DiskExecutionTestsBase
{
    public RemainingPinnedExecutionTests() : base("language.types.boolean") { }

    [Fact(DisplayName = "S8.3_A2.1.js")]
    public Task S8_3_A2_1()
        => CompilationFailureTest("S8.3_A2.1");

    [Fact(DisplayName = "S8.3_A2.2.js")]
    public Task S8_3_A2_2()
        => CompilationFailureTest("S8.3_A2.2");

}
