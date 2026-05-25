using Js2IL.Test262.Tests.language.statements;

namespace Js2IL.Test262.Tests.language.statements.if_;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\statements\if", "language.statements.if_") { }

    [Fact(DisplayName = "S12.5_A10_T1")]
    public Task S12_5_A10_T1()
        => ExecutionTest("S12.5_A10_T1");

    [Fact(DisplayName = "S12.5_A10_T2")]
    public Task S12_5_A10_T2()
        => ExecutionTest("S12.5_A10_T2");

    [Fact(DisplayName = "S12.5_A12_T1")]
    public Task S12_5_A12_T1()
        => ExecutionTest("S12.5_A12_T1");

    [Fact(DisplayName = "S12.5_A4")]
    public Task S12_5_A4()
        => ExecutionTest("S12.5_A4");

    [Fact(DisplayName = "empty-statement")]
    public Task empty_statement()
        => ExecutionTest("empty-statement");
    [Fact(DisplayName = "S12.5_A1_T1")]
    public Task S12_5_A1_T1()
        => ExecutionTest("S12.5_A1_T1");

    [Fact(DisplayName = "S12.5_A1.1_T1")]
    public Task S12_5_A1_1_T1()
        => ExecutionTest("S12.5_A1.1_T1");

    [Fact(DisplayName = "S12.5_A1.2_T1")]
    public Task S12_5_A1_2_T1()
        => ExecutionTest("S12.5_A1.2_T1");



    [Fact(DisplayName = "S12.5_A1.1_T2")]
    public Task S12_5_A1_1_T2()
        => ExecutionTest("S12.5_A1.1_T2");

    [Fact(DisplayName = "S12.5_A1.2_T2")]
    public Task S12_5_A1_2_T2()
        => ExecutionTest("S12.5_A1.2_T2");

    [Fact(DisplayName = "S12.5_A12_T2")]
    public Task S12_5_A12_T2()
        => ExecutionTest("S12.5_A12_T2");

    [Fact(DisplayName = "S12.5_A12_T3")]
    public Task S12_5_A12_T3()
        => ExecutionTest("S12.5_A12_T3");

    [Fact(DisplayName = "S12.5_A12_T4")]
    public Task S12_5_A12_T4()
        => ExecutionTest("S12.5_A12_T4");

    [Fact(DisplayName = "S12.5_A1_T2")]
    public Task S12_5_A1_T2()
        => ExecutionTest("S12.5_A1_T2");

    [Fact(DisplayName = "S12.5_A3")]
    public Task S12_5_A3()
        => ExecutionTest("S12.5_A3");

    [Fact(DisplayName = "S12.5_A5")]
    public Task S12_5_A5()
        => ExecutionTest("S12.5_A5");

    [Fact(DisplayName = "let-block-with-newline")]
    public Task let_block_with_newline()
        => ExecutionTest("let-block-with-newline");
}
