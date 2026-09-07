using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements.class_.elements.async_private_method;

public class ClassSyntaxParseExecutionTests : FileSystemExecutionTestsBase
{
    public ClassSyntaxParseExecutionTests() : base("language/statements/class/elements/async-private-method", "language.statements.class_.elements.async_private_method") { }

    [Fact(DisplayName = "await-as-binding-identifier-escaped")]
    public Task await_as_binding_identifier_escaped()
        => CompilationFailureTest("await-as-binding-identifier-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-as-binding-identifier")]
    public Task await_as_binding_identifier()
        => CompilationFailureTest("await-as-binding-identifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-as-identifier-reference-escaped")]
    public Task await_as_identifier_reference_escaped()
        => CompilationFailureTest("await-as-identifier-reference-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-as-identifier-reference")]
    public Task await_as_identifier_reference()
        => CompilationFailureTest("await-as-identifier-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-as-label-identifier-escaped")]
    public Task await_as_label_identifier_escaped()
        => CompilationFailureTest("await-as-label-identifier-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "await-as-label-identifier")]
    public Task await_as_label_identifier()
        => CompilationFailureTest("await-as-label-identifier", "Failed to parse JavaScript");
}
