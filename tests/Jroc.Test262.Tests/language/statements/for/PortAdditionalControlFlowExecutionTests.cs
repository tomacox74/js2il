using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements.for_;

public class PortAdditionalControlFlowExecutionTests : FileSystemExecutionTestsBase
{
    public PortAdditionalControlFlowExecutionTests() : base(@"language\statements\for", "language.statements.for_") { }

    [Fact(DisplayName = "S12.6.3_A1")]
    public Task S12_6_3_A1()
        => ExecutionTest("S12.6.3_A1");

    [Fact(DisplayName = "S12.6.3_A10_T1")]
    public Task S12_6_3_A10_T1()
        => ExecutionTest("S12.6.3_A10_T1");

    [Fact(DisplayName = "S12.6.3_A10_T2")]
    public Task S12_6_3_A10_T2()
        => ExecutionTest("S12.6.3_A10_T2");

    [Fact(DisplayName = "S12.6.3_A11.1_T1")]
    public Task S12_6_3_A11_1_T1()
        => ExecutionTest("S12.6.3_A11.1_T1");

    [Fact(DisplayName = "S12.6.3_A11.1_T2")]
    public Task S12_6_3_A11_1_T2()
        => ExecutionTest("S12.6.3_A11.1_T2");

    [Fact(DisplayName = "S12.6.3_A11_T1")]
    public Task S12_6_3_A11_T1()
        => ExecutionTest("S12.6.3_A11_T1");

    [Fact(DisplayName = "S12.6.3_A11_T2")]
    public Task S12_6_3_A11_T2()
        => ExecutionTest("S12.6.3_A11_T2");

    [Fact(DisplayName = "S12.6.3_A12.1_T1")]
    public Task S12_6_3_A12_1_T1()
        => ExecutionTest("S12.6.3_A12.1_T1");

    [Fact(DisplayName = "S12.6.3_A12.1_T2")]
    public Task S12_6_3_A12_1_T2()
        => ExecutionTest("S12.6.3_A12.1_T2");

    [Fact(DisplayName = "S12.6.3_A12_T1")]
    public Task S12_6_3_A12_T1()
        => ExecutionTest("S12.6.3_A12_T1");

    [Fact(DisplayName = "S12.6.3_A12_T2")]
    public Task S12_6_3_A12_T2()
        => ExecutionTest("S12.6.3_A12_T2");

    [Fact(DisplayName = "S12.6.3_A13")]
    public Task S12_6_3_A13()
        => ExecutionTest("S12.6.3_A13");

    [Fact(DisplayName = "S12.6.3_A14")]
    public Task S12_6_3_A14()
        => ExecutionTest("S12.6.3_A14");

    [Fact(DisplayName = "S12.6.3_A15")]
    public Task S12_6_3_A15()
        => ExecutionTest("S12.6.3_A15");

    [Fact(DisplayName = "S12.6.3_A2.1")]
    public Task S12_6_3_A2_1()
        => ExecutionTest("S12.6.3_A2.1");

    [Fact(DisplayName = "S12.6.3_A2.2")]
    public Task S12_6_3_A2_2()
        => ExecutionTest("S12.6.3_A2.2");

    [Fact(DisplayName = "S12.6.3_A2")]
    public Task S12_6_3_A2()
        => ExecutionTest("S12.6.3_A2");

    [Fact(DisplayName = "S12.6.3_A3")]
    public Task S12_6_3_A3()
        => ExecutionTest("S12.6.3_A3");

    [Fact(DisplayName = "S12.6.3_A6")]
    public Task S12_6_3_A6()
        => ExecutionTest("S12.6.3_A6");

    [Fact(DisplayName = "head-init-expr-check-empty-inc-empty-syntax")]
    public Task head_init_expr_check_empty_inc_empty_syntax()
        => ExecutionTest("head-init-expr-check-empty-inc-empty-syntax");

    [Fact(DisplayName = "head-init-var-check-empty-inc-empty-syntax")]
    public Task head_init_var_check_empty_inc_empty_syntax()
        => ExecutionTest("head-init-var-check-empty-inc-empty-syntax");

    [Fact(DisplayName = "head-var-bound-names-in-stmt")]
    public Task head_var_bound_names_in_stmt()
        => ExecutionTest("head-var-bound-names-in-stmt");

}
