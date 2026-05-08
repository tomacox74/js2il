using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.const_.syntax;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.const_.syntax") { }

    [Fact(DisplayName = "const-invalid-assignment-next-expression-for")]
    public Task const_invalid_assignment_next_expression_for()
        => ExecutionTest("const-invalid-assignment-next-expression-for");

    [Fact(DisplayName = "const-invalid-assignment-statement-body-for-in")]
    public Task const_invalid_assignment_statement_body_for_in()
        => ExecutionTest("const-invalid-assignment-statement-body-for-in");

    [Fact(DisplayName = "const-invalid-assignment-statement-body-for-of")]
    public Task const_invalid_assignment_statement_body_for_of()
        => ExecutionTest("const-invalid-assignment-statement-body-for-of");

    [Fact(DisplayName = "const-outer-inner-let-bindings")]
    public Task const_outer_inner_let_bindings()
        => ExecutionTest("const-outer-inner-let-bindings");

    [Fact(DisplayName = "const", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task const_()
        => ExecutionTest("const");

    [Fact(DisplayName = "with-initializer-case-expression-statement-list", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task with_initializer_case_expression_statement_list()
        => ExecutionTest("with-initializer-case-expression-statement-list");

    [Fact(DisplayName = "with-initializer-default-statement-list", Skip = "Known issue: runtime behavior diverges from test262 expectation")]
    public Task with_initializer_default_statement_list()
        => ExecutionTest("with-initializer-default-statement-list");
}
