namespace Jroc.Test262.Tests.language.statements.test_class.subclass.builtin_objects.Promise;

public sealed class ClassDefinitionConformanceExecutionTests : DiskExecutionTestsBase
{
    public ClassDefinitionConformanceExecutionTests() : base("language.statements.test_class.subclass.builtin_objects.Promise") { }

    [Fact(DisplayName = "regular-subclassing.js")]
    public Task regular_subclassing()
        => ExecutionTest("regular-subclassing");
}
