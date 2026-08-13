namespace Jroc.Test262.Tests.language.statements.await_using.syntax;

public class Section14_7_5ExecutionTests01 : Jroc.Test262.Tests.language.statements.FileSystemExecutionTestsBase
{
    public Section14_7_5ExecutionTests01() : base("language/statements/await-using/syntax", "language.statements.await_using.syntax") { }

    [Fact(DisplayName = "await-using-invalid-for-in.js")]
    public Task await_using_invalid_for_in_1()
        => CompilationFailureTest("await-using-invalid-for-in", string.Empty);

    [Fact(DisplayName = "await-using-valid-for-await-using-of-of.js", Skip = "Explicit resource management is not supported by JROC.")]
    public Task await_using_valid_for_await_using_of_of_2()
        => ExecutionTest("await-using-valid-for-await-using-of-of");
}
