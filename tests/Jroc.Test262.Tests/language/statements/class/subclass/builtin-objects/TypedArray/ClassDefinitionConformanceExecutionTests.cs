namespace Jroc.Test262.Tests.language.statements.test_class.subclass.builtin_objects.TypedArray;

public sealed class ClassDefinitionConformanceExecutionTests : DiskExecutionTestsBase
{
    public ClassDefinitionConformanceExecutionTests() : base("language.statements.test_class.subclass.builtin_objects.TypedArray") { }

    [Fact(DisplayName = "regular-subclassing.js")]
    public Task regular_subclassing()
        => ExecutionTest("regular-subclassing");

    [Fact(DisplayName = "super-must-be-called.js")]
    public Task super_must_be_called()
        => ExecutionTest("super-must-be-called");
}
