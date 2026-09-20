using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.expressions.tagged_template;

public class Test262BatchPort20260920Round5ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260920Round5ExecutionTests() : base("language.expressions.tagged_template") { }

    [Fact(DisplayName = "cache-different-functions-same-site")]
    public Task cache_different_functions_same_site()
        => ExecutionTest("cache-different-functions-same-site");

    [Fact(DisplayName = "cache-differing-expressions")]
    public Task cache_differing_expressions()
        => ExecutionTest("cache-differing-expressions");

    [Fact(DisplayName = "cache-differing-raw-strings")]
    public Task cache_differing_raw_strings()
        => ExecutionTest("cache-differing-raw-strings");

    [Fact(DisplayName = "cache-differing-string-count")]
    public Task cache_differing_string_count()
        => ExecutionTest("cache-differing-string-count");

    [Fact(DisplayName = "cache-identical-source")]
    public Task cache_identical_source()
        => ExecutionTest("cache-identical-source");

    [Fact(DisplayName = "cache-same-site-top-level")]
    public Task cache_same_site_top_level()
        => ExecutionTest("cache-same-site-top-level");

    [Fact(DisplayName = "cache-same-site")]
    public Task cache_same_site()
        => ExecutionTest("cache-same-site");

    [Fact(DisplayName = "call-expression-context-no-strict")]
    public Task call_expression_context_no_strict()
        => ExecutionTest("call-expression-context-no-strict");

    [Fact(DisplayName = "call-expression-context-strict")]
    public Task call_expression_context_strict()
        => ExecutionTest("call-expression-context-strict");

    [Fact(DisplayName = "constructor-invocation")]
    public Task constructor_invocation()
        => ExecutionTest("constructor-invocation");

    [Fact(DisplayName = "template-object-template-map")]
    public Task template_object_template_map()
        => ExecutionTest("template-object-template-map");

}
