using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.const_;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.const_") { }

    [Fact(DisplayName = "block-local-closure-get-before-initialization", Skip = "Const temporal dead zone behavior for closed-over bindings is incomplete.")]
    public Task block_local_closure_get_before_initialization()
        => ExecutionTest("block-local-closure-get-before-initialization");

    [Fact(DisplayName = "block-local-use-before-initialization-in-declaration-statement", Skip = "Const temporal dead zone self-reference handling is incomplete.")]
    public Task block_local_use_before_initialization_in_declaration_statement()
        => ExecutionTest("block-local-use-before-initialization-in-declaration-statement");
}
