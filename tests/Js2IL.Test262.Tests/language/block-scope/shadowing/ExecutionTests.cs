using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.block_scope.shadowing;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.block_scope.shadowing") { }

    [Fact(DisplayName = "dynamic-lookup-from-closure", Skip = "Product defect: nested closure call lowers to unsupported LIRCallFunction")]
    public Task dynamic_lookup_from_closure()
        => ExecutionTest("dynamic-lookup-from-closure");

    [Fact(DisplayName = "dynamic-lookup-in-and-through-block-contexts")]
    public Task dynamic_lookup_in_and_through_block_contexts()
        => ExecutionTest("dynamic-lookup-in-and-through-block-contexts");

    [Fact(DisplayName = "let-declarations-shadowing-parameter-name-let-const-and-var")]
    public Task let_declarations_shadowing_parameter_name_let_const_and_var()
        => ExecutionTest("let-declarations-shadowing-parameter-name-let-const-and-var");

    [Fact(DisplayName = "lookup-from-closure", Skip = "Product defect: nested closure call lowers to unsupported LIRCallFunction")]
    public Task lookup_from_closure()
        => ExecutionTest("lookup-from-closure");

    [Fact(DisplayName = "lookup-in-and-through-block-contexts")]
    public Task lookup_in_and_through_block_contexts()
        => ExecutionTest("lookup-in-and-through-block-contexts");
}
