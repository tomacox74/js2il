using Jroc.Test262.Tests.language.statements;

namespace Jroc.Test262.Tests.language.statements.switch_;

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
    [Fact(DisplayName = "scope-lex-close-dflt")]
    public Task scope_lex_close_dflt()
        => ExecutionTest("scope-lex-close-dflt");

    [Fact(DisplayName = "scope-lex-open-case")]
    public Task scope_lex_open_case()
        => ExecutionTest("scope-lex-open-case");



    [Fact(DisplayName = "S12.11_A2_T1")]
    public Task S12_11_A2_T1()
        => CompilationFailureTest("S12.11_A2_T1", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.11_A3_T1")]
    public Task S12_11_A3_T1()
        => CompilationFailureTest("S12.11_A3_T1", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.11_A3_T2")]
    public Task S12_11_A3_T2()
        => CompilationFailureTest("S12.11_A3_T2", "Failed to parse JavaScript");

    [Fact(DisplayName = "scope-lex-const")]
    public Task scope_lex_const()
        => ExecutionTest("scope-lex-const", allowUnhandledException: true);

    [Fact(DisplayName = "scope-lex-let")]
    public Task scope_lex_let()
        => ExecutionTest("scope-lex-let", allowUnhandledException: true);

    [Fact(DisplayName = "scope-lex-class")]
    public Task scope_lex_class()
        => ExecutionTest("scope-lex-class", allowUnhandledException: true);

    [Fact(DisplayName = "scope-lex-generator")]
    public Task scope_lex_generator()
        => ExecutionTest("scope-lex-generator", allowUnhandledException: true);

    [Fact(DisplayName = "scope-lex-async-function")]
    public Task scope_lex_async_function()
        => ExecutionTest("scope-lex-async-function", allowUnhandledException: true);

}
