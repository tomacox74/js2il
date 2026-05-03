using Js2IL.Test262.Tests.language.statements;

namespace Js2IL.Test262.Tests.language.statements.switch_;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\statements\switch", "language.statements.switch_") { }

    [Fact(DisplayName = "S12.11_A1_T1")]
    public Task S12_11_A1_T1()
        => ExecutionTest("S12.11_A1_T1");

    [Fact(DisplayName = "S12.11_A1_T2")]
    public Task S12_11_A1_T2()
        => ExecutionTest("S12.11_A1_T2");

    [Fact(DisplayName = "S12.11_A1_T3")]
    public Task S12_11_A1_T3()
        => ExecutionTest("S12.11_A1_T3");

    [Fact(DisplayName = "S12.11_A1_T4")]
    public Task S12_11_A1_T4()
        => ExecutionTest("S12.11_A1_T4");

    [Fact(DisplayName = "S12.11_A4_T1")]
    public Task S12_11_A4_T1()
        => ExecutionTest("S12.11_A4_T1");

    [Fact(DisplayName = "scope-lex-close-case")]
    public Task scope_lex_close_case()
        => ExecutionTest("scope-lex-close-case");

    [Fact(DisplayName = "scope-lex-open-dflt")]
    public Task scope_lex_open_dflt()
        => ExecutionTest("scope-lex-open-dflt");
}
