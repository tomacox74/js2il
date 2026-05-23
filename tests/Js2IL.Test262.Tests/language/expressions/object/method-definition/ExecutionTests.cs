using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.object_.method_definition;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.object_.method_definition") { }

    [Fact(DisplayName = "computed-property-name-yield-expression")]
    public Task computed_property_name_yield_expression()
        => ExecutionTest("computed-property-name-yield-expression");

    [Fact(DisplayName = "fn-name-fn")]
    public Task fn_name_fn()
        => ExecutionTest("fn-name-fn");

    [Fact(DisplayName = "fn-name-gen")]
    public Task fn_name_gen()
        => ExecutionTest("fn-name-gen");

    [Fact(DisplayName = "generator-invoke-ctor")]
    public Task generator_invoke_ctor()
        => ExecutionTest("generator-invoke-ctor");

    [Fact(DisplayName = "generator-invoke-fn-no-strict")]
    public Task generator_invoke_fn_no_strict()
        => ExecutionTest("generator-invoke-fn-no-strict");

    [Fact(DisplayName = "generator-invoke-fn-strict")]
    public Task generator_invoke_fn_strict()
        => ExecutionTest("generator-invoke-fn-strict");
}
