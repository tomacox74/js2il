namespace Jroc.Test262.Tests.language.statements.test_class.subclass.builtin_objects.Error;

public sealed class ClassDefinitionConformanceExecutionTests : DiskExecutionTestsBase
{
    public ClassDefinitionConformanceExecutionTests() : base("language.statements.test_class.subclass.builtin_objects.Error") { }

    [Fact(DisplayName = "message-property-assignment.js")]
    public Task message_property_assignment()
        => ExecutionTest("message-property-assignment");

    [Fact(DisplayName = "regular-subclassing.js")]
    public Task regular_subclassing()
        => ExecutionTest("regular-subclassing");
}
