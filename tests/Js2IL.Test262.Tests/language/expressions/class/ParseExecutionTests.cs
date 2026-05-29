using Js2IL.Test262.Tests.language.statements;

namespace Js2IL.Test262.Tests.language.expressions.class_;

public class ParseExecutionTests : FileSystemExecutionTestsBase
{
    public ParseExecutionTests() : base(@"language\expressions\class", "language.expressions.class_") { }

    [Fact(DisplayName = "class-name-ident-let")]
    public Task class_name_ident_let()
        => CompilationFailureTest("class-name-ident-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-ident-static")]
    public Task class_name_ident_static()
        => CompilationFailureTest("class-name-ident-static", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-ident-yield")]
    public Task class_name_ident_yield()
        => CompilationFailureTest("class-name-ident-yield", "Failed to parse JavaScript");
}
