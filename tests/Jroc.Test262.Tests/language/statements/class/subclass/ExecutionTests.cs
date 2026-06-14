using Jroc.Tests;

namespace Jroc.Test262.Tests.language.statements.class_.subclass;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.subclass") { }

    [Fact(DisplayName = "binding")]
    public Task binding()
        => ExecutionTest("binding");

    [Fact(DisplayName = "class-definition-evaluation-empty-constructor-heritage-present")]
    public Task class_definition_evaluation_empty_constructor_heritage_present()
        => ExecutionTest("class-definition-evaluation-empty-constructor-heritage-present");

    [Fact(DisplayName = "class-definition-null-proto-super")]
    public Task class_definition_null_proto_super()
        => ExecutionTest("class-definition-null-proto-super");

    [Fact(DisplayName = "default-constructor")]
    public Task default_constructor()
        => ExecutionTest("default-constructor");
}
