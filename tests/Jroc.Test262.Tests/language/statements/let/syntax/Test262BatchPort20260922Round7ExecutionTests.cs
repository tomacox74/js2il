using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.statements.let.syntax;

public class Test262BatchPort20260922Round7ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260922Round7ExecutionTests() : base("language.statements.let.syntax") { }

    [Fact(DisplayName = "escaped-let")]
    public Task escaped_let()
        => ExecutionTest("escaped-let");

    [Fact(DisplayName = "let-closure-inside-condition")]
    public Task let_closure_inside_condition()
        => ExecutionTest("let-closure-inside-condition");

    [Fact(DisplayName = "let-closure-inside-initialization")]
    public Task let_closure_inside_initialization()
        => ExecutionTest("let-closure-inside-initialization");

    [Fact(DisplayName = "let-closure-inside-next-expression")]
    public Task let_closure_inside_next_expression()
        => ExecutionTest("let-closure-inside-next-expression");

    [Fact(DisplayName = "let-iteration-variable-is-freshly-allocated-for-each-iteration-multi-let-binding")]
    public Task let_iteration_variable_is_freshly_allocated_for_each_iteration_multi_let_binding()
        => ExecutionTest("let-iteration-variable-is-freshly-allocated-for-each-iteration-multi-let-binding");

    [Fact(DisplayName = "let-iteration-variable-is-freshly-allocated-for-each-iteration-single-let-binding")]
    public Task let_iteration_variable_is_freshly_allocated_for_each_iteration_single_let_binding()
        => ExecutionTest("let-iteration-variable-is-freshly-allocated-for-each-iteration-single-let-binding");

    [Fact(DisplayName = "let-outer-inner-let-bindings")]
    public Task let_outer_inner_let_bindings()
        => ExecutionTest("let-outer-inner-let-bindings");

    [Fact(DisplayName = "let")]
    public Task let()
        => ExecutionTest("let");

    [Fact(DisplayName = "with-initialisers-in-statement-positions-case-expression-statement-list")]
    public Task with_initialisers_in_statement_positions_case_expression_statement_list()
        => ExecutionTest("with-initialisers-in-statement-positions-case-expression-statement-list");

    [Fact(DisplayName = "with-initialisers-in-statement-positions-default-statement-list")]
    public Task with_initialisers_in_statement_positions_default_statement_list()
        => ExecutionTest("with-initialisers-in-statement-positions-default-statement-list");

    [Fact(DisplayName = "without-initialisers-in-statement-positions-case-expression-statement-list")]
    public Task without_initialisers_in_statement_positions_case_expression_statement_list()
        => ExecutionTest("without-initialisers-in-statement-positions-case-expression-statement-list");

    [Fact(DisplayName = "without-initialisers-in-statement-positions-default-statement-list")]
    public Task without_initialisers_in_statement_positions_default_statement_list()
        => ExecutionTest("without-initialisers-in-statement-positions-default-statement-list");

}
