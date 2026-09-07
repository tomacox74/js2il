using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements.class_;

public class ClassSyntaxParseExecutionTests : FileSystemExecutionTestsBase
{
    public ClassSyntaxParseExecutionTests() : base("language/statements/class", "language.statements.class_") { }

    [Fact(DisplayName = "async-gen-meth-escaped-async")]
    public Task async_gen_meth_escaped_async()
        => CompilationFailureTest("async-gen-meth-escaped-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "async-meth-escaped-async")]
    public Task async_meth_escaped_async()
        => CompilationFailureTest("async-meth-escaped-async", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-ident-let-escaped")]
    public Task class_name_ident_let_escaped()
        => CompilationFailureTest("class-name-ident-let-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-ident-let")]
    public Task class_name_ident_let()
        => CompilationFailureTest("class-name-ident-let", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-ident-static-escaped")]
    public Task class_name_ident_static_escaped()
        => CompilationFailureTest("class-name-ident-static-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-ident-static")]
    public Task class_name_ident_static()
        => CompilationFailureTest("class-name-ident-static", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-ident-yield-escaped")]
    public Task class_name_ident_yield_escaped()
        => CompilationFailureTest("class-name-ident-yield-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "class-name-ident-yield")]
    public Task class_name_ident_yield()
        => CompilationFailureTest("class-name-ident-yield", "Failed to parse JavaScript");
}
