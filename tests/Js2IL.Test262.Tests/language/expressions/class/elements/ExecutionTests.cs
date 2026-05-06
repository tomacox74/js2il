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

[Fact(DisplayName = "class-name-static-initializer-expr", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task class_name_static_initializer_expr()
        => ExecutionTest("class-name-static-initializer-expr");

[Fact(DisplayName = "fields-asi-1", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task fields_asi_1()
        => ExecutionTest("fields-asi-1");

[Fact(DisplayName = "class-name-static-initializer-anonymous", Skip = "Class static initializer bodies are not compiled yet.")]
    public Task class_name_static_initializer_anonymous()
        => ExecutionTest("class-name-static-initializer-anonymous");

[Fact(DisplayName = "class-name-static-initializer-decl", Skip = "Class static initializer bodies are not compiled yet.")]
    public Task class_name_static_initializer_decl()
        => ExecutionTest("class-name-static-initializer-decl");

[Fact(DisplayName = "class-name-static-initializer-expr", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task class_name_static_initializer_expr()
        => ExecutionTest("class-name-static-initializer-expr");

[Fact(DisplayName = "fields-asi-1", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task fields_asi_1()
        => ExecutionTest("fields-asi-1");

[Fact(DisplayName = "fields-asi-2", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task fields_asi_2()
        => ExecutionTest("fields-asi-2");

[Fact(DisplayName = "fields-asi-5", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task fields_asi_5()
        => ExecutionTest("fields-asi-5");
}
