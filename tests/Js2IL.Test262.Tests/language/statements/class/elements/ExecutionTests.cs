using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.class_.elements;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.elements") { }

    [Fact(DisplayName = "abrupt-completition-on-field-initializer", Skip = "Class static field initializer bodies are not compiled yet.")]
    public Task abrupt_completition_on_field_initializer()
        => ExecutionTest("abrupt-completition-on-field-initializer");
}
