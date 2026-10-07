namespace Jroc.Test262.Tests.language.statements.class_.subclass.builtin_objects.Date;

public sealed class ConstructorCompletionExecutionTests : DiskExecutionTestsBase
{
    public ConstructorCompletionExecutionTests() : base("Class.ConstructorCompletion") { }

    [Fact(DisplayName = "super-must-be-called.js")]
    public Task super_must_be_called()
        => ExecutionTestFromFile("super-must-be-called");

}
