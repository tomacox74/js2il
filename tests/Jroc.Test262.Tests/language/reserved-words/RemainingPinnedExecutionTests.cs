namespace Jroc.Test262.Tests.language.reserved_words;

public class RemainingPinnedExecutionTests : DiskExecutionTestsBase
{
    public RemainingPinnedExecutionTests() : base("language.reserved_words") { }

    [Fact(DisplayName = "await-module.js")]
    public Task await_module()
        => CompilationFailureTest("await-module");

}
