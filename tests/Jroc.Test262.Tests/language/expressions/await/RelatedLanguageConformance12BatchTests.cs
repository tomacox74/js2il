using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.await_;

public sealed class RelatedLanguageConformance12BatchTests : DiskExecutionTestsBase
{
    public RelatedLanguageConformance12BatchTests() : base("language.expressions.await_") { }

    [Fact(DisplayName = "language/expressions/await/await-BindingIdentifier-in-global.js")]
    public Task test_await_BindingIdentifier_in_global()
        => ExecutionTest("await-BindingIdentifier-in-global");

    [Fact(DisplayName = "language/expressions/await/await-in-function.js")]
    public Task test_await_in_function()
        => ExecutionTest("await-in-function");

    [Fact(DisplayName = "language/expressions/await/await-in-generator.js")]
    public Task test_await_in_generator()
        => ExecutionTest("await-in-generator");

    [Fact(DisplayName = "language/expressions/await/await-in-global.js")]
    public Task test_await_in_global()
        => ExecutionTest("await-in-global");

    [Fact(DisplayName = "language/expressions/await/await-in-nested-function.js")]
    public Task test_await_in_nested_function()
        => ExecutionTest("await-in-nested-function");

    [Fact(DisplayName = "language/expressions/await/await-in-nested-generator.js")]
    public Task test_await_in_nested_generator()
        => ExecutionTest("await-in-nested-generator");
}
