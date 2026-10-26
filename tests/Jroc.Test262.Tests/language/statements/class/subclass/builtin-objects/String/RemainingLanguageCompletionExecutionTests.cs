namespace Jroc.Test262.Tests.language.statements.class_.subclass.builtin_objects.String;

public sealed class RemainingLanguageCompletionExecutionTests : DiskExecutionTestsBase
{
    public RemainingLanguageCompletionExecutionTests() : base("language.statements.class_.subclass.builtin_objects.String") { }

    [Fact(DisplayName = "length.js")]
    public Task length()
        => ExecutionTest("length");

    [Fact(DisplayName = "regular-subclassing.js")]
    public Task regular_subclassing()
        => ExecutionTest("regular-subclassing");
}
