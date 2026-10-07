namespace Jroc.Test262.Tests.language.statements.class_.subclass.builtin_objects.Object;

public sealed class ConstructorCompletionExecutionTests : DiskExecutionTestsBase
{
    public ConstructorCompletionExecutionTests() : base("Class.ConstructorCompletion") { }

    [Fact(DisplayName = "constructor-return-undefined-throws.js")]
    public Task constructor_return_undefined_throws()
        => ExecutionTestFromFile("constructor-return-undefined-throws");

    [Fact(DisplayName = "constructor-returns-non-object.js")]
    public Task constructor_returns_non_object()
        => ExecutionTestFromFile("constructor-returns-non-object");

}
