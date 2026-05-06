using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.class_.elements;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.elements") { }

[Fact(DisplayName = "abrupt-completition-on-field-initializer", Skip = "Class static field initializer bodies are not compiled yet.")]
    public Task abrupt_completition_on_field_initializer()
        => ExecutionTest("abrupt-completition-on-field-initializer");

[Fact(DisplayName = "class-field-is-observable-by-proxy", Skip = "Known issue: compiler cannot yet compile this test262 scenario")]
    public Task class_field_is_observable_by_proxy()
        => ExecutionTest("class-field-is-observable-by-proxy");
}
