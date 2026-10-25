namespace Jroc.Test262.Tests.language.statements.test_class.subclass.builtin_objects.GeneratorFunction;

public sealed class RemainingLanguageCompletionExecutionTests : DiskExecutionTestsBase
{
    public RemainingLanguageCompletionExecutionTests() : base("language.statements.test_class.subclass.builtin_objects.GeneratorFunction") { }

    [Fact(DisplayName = "regular-subclassing.js")]
    public Task regular_subclassing()
        => ExecutionTest("regular-subclassing");
}
