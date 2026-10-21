namespace Jroc.Test262.Tests.language.statements.test_class.subclass.builtin_objects.DataView;

public sealed class ClassDefinitionConformanceExecutionTests : DiskExecutionTestsBase
{
    public ClassDefinitionConformanceExecutionTests() : base("language.statements.test_class.subclass.builtin_objects.DataView") { }

    [Fact(DisplayName = "regular-subclassing.js")]
    public Task regular_subclassing()
        => ExecutionTest("regular-subclassing");
}
