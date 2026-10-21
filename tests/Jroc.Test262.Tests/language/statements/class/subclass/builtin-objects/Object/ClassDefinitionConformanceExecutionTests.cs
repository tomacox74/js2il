namespace Jroc.Test262.Tests.language.statements.test_class.subclass.builtin_objects.Object;

public sealed class ClassDefinitionConformanceExecutionTests : DiskExecutionTestsBase
{
    public ClassDefinitionConformanceExecutionTests() : base("language.statements.test_class.subclass.builtin_objects.Object") { }

    [Fact(DisplayName = "regular-subclassing.js")]
    public Task regular_subclassing()
        => ExecutionTest("regular-subclassing");

    [Fact(DisplayName = "replacing-prototype.js")]
    public Task replacing_prototype()
        => ExecutionTest("replacing-prototype");
}
