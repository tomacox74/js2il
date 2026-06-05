using Js2IL.Test262.Tests.language.statements;

namespace Js2IL.Test262.Tests.language.statements.block;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base(@"language\statements\block", "language.statements.block") { }

    [Fact(DisplayName = "S12.1_A2")]
    public Task S12_1_A2()
        => ExecutionTest("S12.1_A2");

    [Fact(DisplayName = "S12.1_A5")]
    public Task S12_1_A5()
        => ExecutionTest("S12.1_A5");

    [Fact(DisplayName = "scope-lex-close")]
    public Task scope_lex_close()
        => ExecutionTest("scope-lex-close");

    [Fact(DisplayName = "scope-lex-open")]
    public Task scope_lex_open()
        => ExecutionTest("scope-lex-open");

    [Fact(DisplayName = "scope-var-none")]
    public Task scope_var_none()
        => ExecutionTest("scope-var-none");
    [Fact(DisplayName = "12.1-1")]
    public Task _12_1_1()
        => CompilationFailureTest("12.1-1", "Failed to parse JavaScript");

    [Fact(DisplayName = "12.1-2")]
    public Task _12_1_2()
        => CompilationFailureTest("12.1-2", "Failed to parse JavaScript");

    [Fact(DisplayName = "12.1-3")]
    public Task _12_1_3()
        => CompilationFailureTest("12.1-3", "Failed to parse JavaScript");

    [Fact(DisplayName = "12.1-4")]
    public Task _12_1_4()
        => CompilationFailureTest("12.1-4", "Failed to parse JavaScript");

    [Fact(DisplayName = "12.1-5")]
    public Task _12_1_5()
        => CompilationFailureTest("12.1-5", "Failed to parse JavaScript");

    [Fact(DisplayName = "12.1-6")]
    public Task _12_1_6()
        => CompilationFailureTest("12.1-6", "Failed to parse JavaScript");

    [Fact(DisplayName = "12.1-7")]
    public Task _12_1_7()
        => CompilationFailureTest("12.1-7", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.1_A4_T1")]
    public Task S12_1_A4_T1()
        => CompilationFailureTest("S12.1_A4_T1", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.1_A4_T2")]
    public Task S12_1_A4_T2()
        => CompilationFailureTest("S12.1_A4_T2", "Failed to parse JavaScript");

    [Fact(DisplayName = "labeled-continue")]
    public Task labeled_continue()
        => CompilationFailureTest("labeled-continue", "Failed to parse JavaScript");

}
