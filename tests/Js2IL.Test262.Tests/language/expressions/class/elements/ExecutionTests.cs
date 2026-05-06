using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.class_.elements;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.class_.elements") { }

    [Fact(DisplayName = "class-name-static-initializer-anonymous", Skip = "Class static initializer bodies are not compiled yet.")]
    public Task class_name_static_initializer_anonymous()
        => ExecutionTest("class-name-static-initializer-anonymous");

    [Fact(DisplayName = "class-name-static-initializer-decl", Skip = "Class static initializer bodies are not compiled yet.")]
    public Task class_name_static_initializer_decl()
        => ExecutionTest("class-name-static-initializer-decl");
}
