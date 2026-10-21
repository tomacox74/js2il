namespace Jroc.Test262.Tests.language.statements.test_class.subclass.builtin_objects.RegExp;

public sealed class ClassDefinitionConformanceExecutionTests : DiskExecutionTestsBase
{
    public ClassDefinitionConformanceExecutionTests() : base("language.statements.test_class.subclass.builtin_objects.RegExp") { }

    [Fact(DisplayName = "lastIndex.js")]
    public Task lastIndex()
        => ExecutionTest("lastIndex");

    [Fact(DisplayName = "regular-subclassing.js")]
    public Task regular_subclassing()
        => ExecutionTest("regular-subclassing");
}
