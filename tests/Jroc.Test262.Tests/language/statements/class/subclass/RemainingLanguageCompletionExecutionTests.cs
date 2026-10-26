namespace Jroc.Test262.Tests.language.statements.test_class.subclass;

public sealed class RemainingLanguageCompletionExecutionTests : DiskExecutionTestsBase
{
    public RemainingLanguageCompletionExecutionTests() : base("language.statements.test_class.subclass") { }

    [Fact(DisplayName = "private-class-field-on-nonextensible-return-override.js", Skip = "Requires the unsupported nonextensible-applies-to-private proposal; ES2025 and Node.js permit private elements on non-extensible objects.")]
    public Task private_class_field_on_nonextensible_return_override()
        => ExecutionTest("private-class-field-on-nonextensible-return-override");

    [Fact(DisplayName = "superclass-bound-function.js")]
    public Task superclass_bound_function()
        => ExecutionTest("superclass-bound-function");
}
