namespace Jroc.Test262.Tests.language.statements.using_.syntax;

public class Section14_7_5ExecutionTests01 : Jroc.Test262.Tests.language.statements.FileSystemExecutionTestsBase
{
    public Section14_7_5ExecutionTests01() : base("language/statements/using/syntax", "language.statements.using_.syntax") { }

    [Fact(DisplayName = "using-for-using-of-of.js")]
    public Task using_for_using_of_of_1()
        => ExecutionTest("using-for-using-of-of");

    [Fact(DisplayName = "using-invalid-for-in.js")]
    public Task using_invalid_for_in_2()
        => CompilationFailureTest("using-invalid-for-in", string.Empty);
}
