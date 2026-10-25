namespace Jroc.Test262.Tests.language.statements.class_.subclass.builtin_objects.Number;

public sealed class RemainingLanguageCompletionExecutionTests : DiskExecutionTestsBase
{
    public RemainingLanguageCompletionExecutionTests() : base("language.statements.class_.subclass.builtin_objects.Number") { }

    [Fact(DisplayName = "regular-subclassing.js")]
    public Task regular_subclassing()
        => ExecutionTest("regular-subclassing");
}
