using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.object_.method_definition;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.object_.method_definition") { }

[Fact(DisplayName = "computed-property-name-yield-expression", Skip = "Computed object property names with yield are not compiled correctly yet.")]
    public Task computed_property_name_yield_expression()
        => ExecutionTest("computed-property-name-yield-expression");

[Fact(DisplayName = "fn-name-fn", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task fn_name_fn()
        => ExecutionTest("fn-name-fn");

[Fact(DisplayName = "computed-property-name-yield-expression", Skip = "Computed object property names with yield are not compiled correctly yet.")]
    public Task computed_property_name_yield_expression()
        => ExecutionTest("computed-property-name-yield-expression");

[Fact(DisplayName = "fn-name-fn", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task fn_name_fn()
        => ExecutionTest("fn-name-fn");

[Fact(DisplayName = "fn-name-gen", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task fn_name_gen()
        => ExecutionTest("fn-name-gen");

[Fact(DisplayName = "generator-invoke-ctor", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task generator_invoke_ctor()
        => ExecutionTest("generator-invoke-ctor");

[Fact(DisplayName = "computed-property-name-yield-expression", Skip = "Computed object property names with yield are not compiled correctly yet.")]
    public Task computed_property_name_yield_expression()
        => ExecutionTest("computed-property-name-yield-expression");

[Fact(DisplayName = "fn-name-fn", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task fn_name_fn()
        => ExecutionTest("fn-name-fn");

[Fact(DisplayName = "fn-name-gen", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task fn_name_gen()
        => ExecutionTest("fn-name-gen");

[Fact(DisplayName = "generator-invoke-ctor", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task generator_invoke_ctor()
        => ExecutionTest("generator-invoke-ctor");

[Fact(DisplayName = "generator-invoke-fn-no-strict", Skip = "Known issue: unstable timeout in this test262 scenario")]
    public Task generator_invoke_fn_no_strict()
        => ExecutionTest("generator-invoke-fn-no-strict");

[Fact(DisplayName = "generator-invoke-fn-strict", Skip = "Known issue: unstable timeout in this test262 scenario")]
    public Task generator_invoke_fn_strict()
        => ExecutionTest("generator-invoke-fn-strict");
}
