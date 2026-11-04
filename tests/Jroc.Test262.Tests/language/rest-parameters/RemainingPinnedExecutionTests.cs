namespace Jroc.Test262.Tests.language.rest_parameters;

public class RemainingPinnedExecutionTests : DiskExecutionTestsBase
{
    public RemainingPinnedExecutionTests() : base("language.rest_parameters") { }

    [Fact(DisplayName = "position-invalid.js")]
    public Task position_invalid()
        => CompilationFailureTest("position-invalid");

    [Fact(DisplayName = "with-new-target.js")]
    public Task with_new_target()
        => ExecutionTest("with-new-target");

}
