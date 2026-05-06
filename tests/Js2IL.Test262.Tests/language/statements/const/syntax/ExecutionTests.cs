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
}
