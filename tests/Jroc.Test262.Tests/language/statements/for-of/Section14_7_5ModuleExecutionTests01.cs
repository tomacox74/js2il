namespace Jroc.Test262.Tests.language.statements.for_of;

public class Section14_7_5ModuleExecutionTests01 : Jroc.Test262.Tests.language.modules.FileSystemExecutionTestsBase
{
    public Section14_7_5ModuleExecutionTests01() : base("language/statements/for-of", "language.statements.for_of") { }

    [Fact(DisplayName = "head-await-using-bound-names-in-stmt.js")]
    public Task head_await_using_bound_names_in_stmt_1()
        => CompilationFailureTest("head-await-using-bound-names-in-stmt", string.Empty);

    [Fact(DisplayName = "head-await-using-fresh-binding-per-iteration.js", Skip = "Explicit resource management is not supported by JROC.")]
    public Task head_await_using_fresh_binding_per_iteration_2()
        => ExecutionTest("head-await-using-fresh-binding-per-iteration");
}
