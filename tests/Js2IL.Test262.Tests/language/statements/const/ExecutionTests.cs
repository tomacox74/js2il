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

    [Fact(DisplayName = "block-local-use-before-initialization-in-prior-statement")]
    public Task block_local_use_before_initialization_in_prior_statement()
        => ExecutionTest("block-local-use-before-initialization-in-prior-statement");

    [Fact(DisplayName = "cptn-value", Skip = "Known JS2IL compiler/runtime limitation")]
    public Task cptn_value()
        => ExecutionTest("cptn-value");

    [Fact(DisplayName = "fn-name-arrow", Skip = "Product gap: inferred name for const-bound anonymous functions is incomplete.")]
    public Task fn_name_arrow()
        => ExecutionTest("fn-name-arrow");

    [Fact(DisplayName = "fn-name-class", Skip = "Product gap: inferred name for const-bound anonymous classes is incomplete.")]
    public Task fn_name_class()
        => ExecutionTest("fn-name-class");

    [Fact(DisplayName = "fn-name-cover", Skip = "Product gap: inferred name for const-bound covered functions is incomplete.")]
    public Task fn_name_cover()
        => ExecutionTest("fn-name-cover");

    [Fact(DisplayName = "fn-name-fn", Skip = "Product gap: inferred name for const-bound anonymous functions is incomplete.")]
    public Task fn_name_fn()
        => ExecutionTest("fn-name-fn");

    [Fact(DisplayName = "fn-name-gen", Skip = "Known issue: runtime behavior diverges from test262 expectation")]
    public Task fn_name_gen()
        => ExecutionTest("fn-name-gen");
}
