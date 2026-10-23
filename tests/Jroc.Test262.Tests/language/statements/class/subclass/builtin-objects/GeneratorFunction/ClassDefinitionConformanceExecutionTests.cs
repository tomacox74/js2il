namespace Jroc.Test262.Tests.language.statements.test_class.subclass.builtin_objects.GeneratorFunction;

public sealed class ClassDefinitionConformanceExecutionTests : DiskExecutionTestsBase
{
    public ClassDefinitionConformanceExecutionTests() : base("language.statements.test_class.subclass.builtin_objects.GeneratorFunction") { }

    [Fact(DisplayName = "instance-length.js")]
    public Task instance_length()
        => ExecutionTest("instance-length");

    [Fact(DisplayName = "instance-name.js")]
    public Task instance_name()
        => ExecutionTest("instance-name");

    [Fact(DisplayName = "instance-prototype.js")]
    public Task instance_prototype()
        => ExecutionTest("instance-prototype");

    [Fact(DisplayName = "super-must-be-called.js")]
    public Task super_must_be_called()
        => ExecutionTest("super-must-be-called");
}
