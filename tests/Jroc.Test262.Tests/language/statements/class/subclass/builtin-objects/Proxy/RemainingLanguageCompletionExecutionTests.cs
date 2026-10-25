namespace Jroc.Test262.Tests.language.statements.Test_class.subclass.builtin_objects.Proxy;

public sealed class RemainingLanguageCompletionExecutionTests : DiskExecutionTestsBase
{
    public RemainingLanguageCompletionExecutionTests() : base("language.statements.Test_class.subclass.builtin_objects.Proxy") { }

    [Fact(DisplayName = "no-prototype-throws.js")]
    public Task no_prototype_throws()
        => ExecutionTest("no-prototype-throws");
}
