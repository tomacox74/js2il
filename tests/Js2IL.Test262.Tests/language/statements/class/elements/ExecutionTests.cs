using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.class_.elements;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.elements") { }

    [Fact(DisplayName = "abrupt-completition-on-field-initializer")]
    public Task abrupt_completition_on_field_initializer()
        => ExecutionTest("abrupt-completition-on-field-initializer");

    [Fact(DisplayName = "class-field-is-observable-by-proxy", Skip = "Tracked by issue #1055: function-base super() and proxy field initialization semantics are incomplete.")]
    public Task class_field_is_observable_by_proxy()
        => ExecutionTest("class-field-is-observable-by-proxy");
}
