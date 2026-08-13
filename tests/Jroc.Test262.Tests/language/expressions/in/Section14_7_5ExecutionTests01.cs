namespace Jroc.Test262.Tests.language.expressions.in_;

public class Section14_7_5ExecutionTests01 : Jroc.Test262.Tests.language.statements.FileSystemExecutionTestsBase
{
    public Section14_7_5ExecutionTests01() : base("language/expressions/in", "language.expressions.in") { }

    [Fact(DisplayName = "private-field-invalid-assignment-reference.js")]
    public Task private_field_invalid_assignment_reference_1()
        => CompilationFailureTest("private-field-invalid-assignment-reference", string.Empty);
}
