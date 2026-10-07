namespace Jroc.Test262.Tests.language.statements.class_.subclass;

public sealed class ConstructorCompletionExecutionTests : DiskExecutionTestsBase
{
    public ConstructorCompletionExecutionTests() : base("Class.ConstructorCompletion") { }

    [Fact(DisplayName = "class-definition-null-proto-contains-return-override.js")]
    public Task class_definition_null_proto_contains_return_override()
        => ExecutionTestFromFile("class-definition-null-proto-contains-return-override");

    [Fact(DisplayName = "class-definition-null-proto-this.js")]
    public Task class_definition_null_proto_this()
        => ExecutionTestFromFile("class-definition-null-proto-this");

    [Fact(DisplayName = "derived-class-return-override-for-of.js")]
    public Task derived_class_return_override_for_of()
        => ExecutionTestFromFile("derived-class-return-override-for-of");

    [Fact(DisplayName = "derived-class-return-override-with-object.js")]
    public Task derived_class_return_override_with_object()
        => ExecutionTestFromFile("derived-class-return-override-with-object");

    [Fact(DisplayName = "derived-class-return-override-with-this.js")]
    public Task derived_class_return_override_with_this()
        => ExecutionTestFromFile("derived-class-return-override-with-this");

    [Fact(DisplayName = "derived-class-return-override-with-undefined.js")]
    public Task derived_class_return_override_with_undefined()
        => ExecutionTestFromFile("derived-class-return-override-with-undefined");

}
