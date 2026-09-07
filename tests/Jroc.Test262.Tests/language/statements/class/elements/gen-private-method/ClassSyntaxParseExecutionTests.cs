using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements.class_.elements.gen_private_method;

public class ClassSyntaxParseExecutionTests : FileSystemExecutionTestsBase
{
    public ClassSyntaxParseExecutionTests() : base("language/statements/class/elements/gen-private-method", "language.statements.class_.elements.gen_private_method") { }

    [Fact(DisplayName = "yield-as-binding-identifier-escaped")]
    public Task yield_as_binding_identifier_escaped()
        => CompilationFailureTest("yield-as-binding-identifier-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-binding-identifier")]
    public Task yield_as_binding_identifier()
        => CompilationFailureTest("yield-as-binding-identifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-identifier-reference-escaped")]
    public Task yield_as_identifier_reference_escaped()
        => CompilationFailureTest("yield-as-identifier-reference-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-identifier-reference")]
    public Task yield_as_identifier_reference()
        => CompilationFailureTest("yield-as-identifier-reference", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-label-identifier-escaped")]
    public Task yield_as_label_identifier_escaped()
        => CompilationFailureTest("yield-as-label-identifier-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-as-label-identifier")]
    public Task yield_as_label_identifier()
        => CompilationFailureTest("yield-as-label-identifier", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-identifier-spread-strict")]
    public Task yield_identifier_spread_strict()
        => CompilationFailureTest("yield-identifier-spread-strict", "Failed to parse JavaScript");

    [Fact(DisplayName = "yield-identifier-strict")]
    public Task yield_identifier_strict()
        => CompilationFailureTest("yield-identifier-strict", "Failed to parse JavaScript");
}
