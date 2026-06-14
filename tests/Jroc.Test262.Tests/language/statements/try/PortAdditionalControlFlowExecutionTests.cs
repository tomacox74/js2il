using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements.try_;

public class PortAdditionalControlFlowExecutionTests : FileSystemExecutionTestsBase
{
    public PortAdditionalControlFlowExecutionTests() : base(@"language\statements\try", "language.statements.try_") { }

    [Fact(DisplayName = "S12.14_A10_T5")]
    public Task S12_14_A10_T5()
        => ExecutionTest("S12.14_A10_T5");

    [Fact(DisplayName = "S12.14_A13_T1")]
    public Task S12_14_A13_T1()
        => ExecutionTest("S12.14_A13_T1");

    [Fact(DisplayName = "S12.14_A13_T2")]
    public Task S12_14_A13_T2()
        => ExecutionTest("S12.14_A13_T2");

    [Fact(DisplayName = "S12.14_A13_T3")]
    public Task S12_14_A13_T3()
        => ExecutionTest("S12.14_A13_T3");

    [Fact(DisplayName = "S12.14_A17")]
    public Task S12_14_A17()
        => ExecutionTest("S12.14_A17");

    [Fact(DisplayName = "S12.14_A18_T1")]
    public Task S12_14_A18_T1()
        => ExecutionTest("S12.14_A18_T1");

    [Fact(DisplayName = "S12.14_A18_T2")]
    public Task S12_14_A18_T2()
        => ExecutionTest("S12.14_A18_T2");

    [Fact(DisplayName = "S12.14_A18_T3")]
    public Task S12_14_A18_T3()
        => ExecutionTest("S12.14_A18_T3");

    [Fact(DisplayName = "S12.14_A18_T4")]
    public Task S12_14_A18_T4()
        => ExecutionTest("S12.14_A18_T4");

    [Fact(DisplayName = "S12.14_A18_T5")]
    public Task S12_14_A18_T5()
        => ExecutionTest("S12.14_A18_T5");

    [Fact(DisplayName = "S12.14_A18_T7")]
    public Task S12_14_A18_T7()
        => ExecutionTest("S12.14_A18_T7");

    [Fact(DisplayName = "S12.14_A19_T1")]
    public Task S12_14_A19_T1()
        => ExecutionTest("S12.14_A19_T1");

    [Fact(DisplayName = "S12.14_A19_T2")]
    public Task S12_14_A19_T2()
        => ExecutionTest("S12.14_A19_T2");

    [Fact(DisplayName = "S12.14_A7_T3")]
    public Task S12_14_A7_T3()
        => ExecutionTest("S12.14_A7_T3");

    [Fact(DisplayName = "optional-catch-binding-finally")]
    public Task optional_catch_binding_finally()
        => ExecutionTest("optional-catch-binding-finally");

    [Fact(DisplayName = "optional-catch-binding-throws")]
    public Task optional_catch_binding_throws()
        => ExecutionTest("optional-catch-binding-throws");

    [Fact(DisplayName = "optional-catch-binding")]
    public Task optional_catch_binding()
        => ExecutionTest("optional-catch-binding");

    [Fact(DisplayName = "scope-catch-block-var-none")]
    public Task scope_catch_block_var_none()
        => ExecutionTest("scope-catch-block-var-none");

}
