using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.arrow_function;

public sealed class RelatedLanguageConformance12BatchTests : DiskExecutionTestsBase
{
    public RelatedLanguageConformance12BatchTests() : base("language.expressions.arrow_function") { }

    [Fact(DisplayName = "language/expressions/arrow-function/scope-param-rest-elem-var-close.js")]
    public Task test_scope_param_rest_elem_var_close()
        => ExecutionTest("scope-param-rest-elem-var-close");

    [Fact(DisplayName = "language/expressions/arrow-function/scope-param-rest-elem-var-open.js")]
    public Task test_scope_param_rest_elem_var_open()
        => ExecutionTest("scope-param-rest-elem-var-open");
}
