using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.block_scope.shadowing;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.block_scope.shadowing") { }

[Fact(DisplayName = "dynamic-lookup-from-closure")]
    public Task dynamic_lookup_from_closure()
        => ExecutionTest("dynamic-lookup-from-closure");

[Fact(DisplayName = "dynamic-lookup-in-and-through-block-contexts")]
    public Task dynamic_lookup_in_and_through_block_contexts()
        => ExecutionTest("dynamic-lookup-in-and-through-block-contexts");

[Fact(DisplayName = "let-declarations-shadowing-parameter-name-let-const-and-var")]
    public Task let_declarations_shadowing_parameter_name_let_const_and_var()
        => ExecutionTest("let-declarations-shadowing-parameter-name-let-const-and-var");

[Fact(DisplayName = "lookup-from-closure")]
    public Task lookup_from_closure()
        => ExecutionTest("lookup-from-closure");

[Fact(DisplayName = "lookup-in-and-through-block-contexts")]
    public Task lookup_in_and_through_block_contexts()
        => ExecutionTest("lookup-in-and-through-block-contexts");

[Fact(DisplayName = "catch-parameter-shadowing-catch-parameter")]
    public Task catch_parameter_shadowing_catch_parameter()
        => ExecutionTest("catch-parameter-shadowing-catch-parameter");

[Fact(DisplayName = "catch-parameter-shadowing-function-parameter-name")]
    public Task catch_parameter_shadowing_function_parameter_name()
        => ExecutionTest("catch-parameter-shadowing-function-parameter-name");

[Fact(DisplayName = "dynamic-lookup-from-closure")]
    public Task dynamic_lookup_from_closure()
        => ExecutionTest("dynamic-lookup-from-closure");

[Fact(DisplayName = "dynamic-lookup-in-and-through-block-contexts")]
    public Task dynamic_lookup_in_and_through_block_contexts()
        => ExecutionTest("dynamic-lookup-in-and-through-block-contexts");

[Fact(DisplayName = "let-declarations-shadowing-parameter-name-let-const-and-var")]
    public Task let_declarations_shadowing_parameter_name_let_const_and_var()
        => ExecutionTest("let-declarations-shadowing-parameter-name-let-const-and-var");

[Fact(DisplayName = "lookup-from-closure")]
    public Task lookup_from_closure()
        => ExecutionTest("lookup-from-closure");

[Fact(DisplayName = "lookup-in-and-through-block-contexts")]
    public Task lookup_in_and_through_block_contexts()
        => ExecutionTest("lookup-in-and-through-block-contexts");

[Fact(DisplayName = "catch-parameter-shadowing-catch-parameter")]
    public Task catch_parameter_shadowing_catch_parameter()
        => ExecutionTest("catch-parameter-shadowing-catch-parameter");

[Fact(DisplayName = "catch-parameter-shadowing-function-parameter-name")]
    public Task catch_parameter_shadowing_function_parameter_name()
        => ExecutionTest("catch-parameter-shadowing-function-parameter-name");

[Fact(DisplayName = "catch-parameter-shadowing-let-declaration", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task catch_parameter_shadowing_let_declaration()
        => ExecutionTest("catch-parameter-shadowing-let-declaration");

[Fact(DisplayName = "dynamic-lookup-from-closure")]
    public Task dynamic_lookup_from_closure()
        => ExecutionTest("dynamic-lookup-from-closure");

[Fact(DisplayName = "dynamic-lookup-in-and-through-block-contexts")]
    public Task dynamic_lookup_in_and_through_block_contexts()
        => ExecutionTest("dynamic-lookup-in-and-through-block-contexts");

[Fact(DisplayName = "let-declarations-shadowing-parameter-name-let-const-and-var")]
    public Task let_declarations_shadowing_parameter_name_let_const_and_var()
        => ExecutionTest("let-declarations-shadowing-parameter-name-let-const-and-var");

[Fact(DisplayName = "lookup-from-closure")]
    public Task lookup_from_closure()
        => ExecutionTest("lookup-from-closure");

[Fact(DisplayName = "lookup-in-and-through-block-contexts")]
    public Task lookup_in_and_through_block_contexts()
        => ExecutionTest("lookup-in-and-through-block-contexts");

[Fact(DisplayName = "catch-parameter-shadowing-catch-parameter")]
    public Task catch_parameter_shadowing_catch_parameter()
        => ExecutionTest("catch-parameter-shadowing-catch-parameter");

[Fact(DisplayName = "catch-parameter-shadowing-function-parameter-name")]
    public Task catch_parameter_shadowing_function_parameter_name()
        => ExecutionTest("catch-parameter-shadowing-function-parameter-name");

[Fact(DisplayName = "catch-parameter-shadowing-let-declaration", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task catch_parameter_shadowing_let_declaration()
        => ExecutionTest("catch-parameter-shadowing-let-declaration");

[Fact(DisplayName = "catch-parameter-shadowing-var-variable")]
    public Task catch_parameter_shadowing_var_variable()
        => ExecutionTest("catch-parameter-shadowing-var-variable");

[Fact(DisplayName = "const-declaration-shadowing-catch-parameter")]
    public Task const_declaration_shadowing_catch_parameter()
        => ExecutionTest("const-declaration-shadowing-catch-parameter");
}
