using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.expressions.class_;

public class ClassSyntaxParseExecutionTests : FileSystemExecutionTestsBase
{
    public ClassSyntaxParseExecutionTests() : base("language/expressions/class", "language.expressions.class_") { }

    [Fact(DisplayName = "class-name-ident-let-escaped")]
    public Task class_name_ident_let_escaped()
        => CompilationFailureTest("class-name-ident-let-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-ident-static-escaped")]
    public Task class_name_ident_static_escaped()
        => CompilationFailureTest("class-name-ident-static-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-ident-yield-escaped")]
    public Task class_name_ident_yield_escaped()
        => CompilationFailureTest("class-name-ident-yield-escaped", "Failed to parse JavaScript");
}
