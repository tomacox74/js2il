using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.block_scope.shadowing;

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

    [Fact(DisplayName = "catch-parameter-shadowing-let-declaration")]
    public Task catch_parameter_shadowing_let_declaration()
        => ExecutionTest("catch-parameter-shadowing-let-declaration");

    [Fact(DisplayName = "catch-parameter-shadowing-var-variable")]
    public Task catch_parameter_shadowing_var_variable()
        => ExecutionTest("catch-parameter-shadowing-var-variable");

    [Fact(DisplayName = "const-declaration-shadowing-catch-parameter")]
    public Task const_declaration_shadowing_catch_parameter()
        => ExecutionTest("const-declaration-shadowing-catch-parameter");
    [Fact(DisplayName = "const-declarations-shadowing-parameter-name-let-const-and-var-variables")]
    public Task const_declarations_shadowing_parameter_name_let_const_and_var_variables()
        => ExecutionTest("const-declarations-shadowing-parameter-name-let-const-and-var-variables");

    [Fact(DisplayName = "hoisting-var-declarations-out-of-blocks")]
    public Task hoisting_var_declarations_out_of_blocks()
        => ExecutionTest("hoisting-var-declarations-out-of-blocks");

    [Fact(DisplayName = "let-declaration-shadowing-catch-parameter")]
    public Task let_declaration_shadowing_catch_parameter()
        => ExecutionTest("let-declaration-shadowing-catch-parameter");

    [Fact(DisplayName = "parameter-name-shadowing-catch-parameter")]
    public Task parameter_name_shadowing_catch_parameter()
        => ExecutionTest("parameter-name-shadowing-catch-parameter");

    [Fact(DisplayName = "parameter-name-shadowing-parameter-name-let-const-and-var")]
    public Task parameter_name_shadowing_parameter_name_let_const_and_var()
        => ExecutionTest("parameter-name-shadowing-parameter-name-let-const-and-var");

}
