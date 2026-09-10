using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.async_function;

public sealed class RelatedLanguageConformance12BatchTests : DiskExecutionTestsBase
{
    public RelatedLanguageConformance12BatchTests() : base("language.expressions.async_function") { }

    [Fact(DisplayName = "language/expressions/async-function/early-errors-expression-binding-identifier-eval.js")]
    public Task test_early_errors_expression_binding_identifier_eval()
        => CompilationFailureTest("early-errors-expression-binding-identifier-eval", "Failed to parse JavaScript");
}
