namespace Jroc.Test262.Tests.language.statements.class_.subclass.builtin_objects.Function;

public sealed class RemainingLanguageCompletionExecutionTests : DiskExecutionTestsBase
{
    public RemainingLanguageCompletionExecutionTests() : base("language.statements.class_.subclass.builtin_objects.Function") { }

    [Fact(DisplayName = "instance-length.js")]
    public Task instance_length()
        => ExecutionTest("instance-length");

    [Fact(DisplayName = "instance-name.js")]
    public Task instance_name()
        => ExecutionTest("instance-name");

    [Fact(DisplayName = "regular-subclassing.js")]
    public Task regular_subclassing()
        => ExecutionTest("regular-subclassing");
}
