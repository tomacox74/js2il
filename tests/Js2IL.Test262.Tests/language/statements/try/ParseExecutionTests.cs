using Js2IL.Test262.Tests.language.statements;

namespace Js2IL.Test262.Tests.language.statements.try_;

public class ParseExecutionTests : FileSystemExecutionTestsBase
{
    public ParseExecutionTests() : base(@"language\statements\try", "language.statements.try_") { }

    [Fact(DisplayName = "dstr/ary-ptrn-rest-init-ary")]
    public Task dstr_ary_ptrn_rest_init_ary()
        => CompilationFailureTest("dstr/ary-ptrn-rest-init-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "dstr/ary-ptrn-rest-init-id")]
    public Task dstr_ary_ptrn_rest_init_id()
        => CompilationFailureTest("dstr/ary-ptrn-rest-init-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "dstr/ary-ptrn-rest-init-obj")]
    public Task dstr_ary_ptrn_rest_init_obj()
        => CompilationFailureTest("dstr/ary-ptrn-rest-init-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "dstr/ary-ptrn-rest-not-final-ary")]
    public Task dstr_ary_ptrn_rest_not_final_ary()
        => CompilationFailureTest("dstr/ary-ptrn-rest-not-final-ary", "Failed to parse JavaScript");

    [Fact(DisplayName = "dstr/ary-ptrn-rest-not-final-id")]
    public Task dstr_ary_ptrn_rest_not_final_id()
        => CompilationFailureTest("dstr/ary-ptrn-rest-not-final-id", "Failed to parse JavaScript");

    [Fact(DisplayName = "dstr/ary-ptrn-rest-not-final-obj")]
    public Task dstr_ary_ptrn_rest_not_final_obj()
        => CompilationFailureTest("dstr/ary-ptrn-rest-not-final-obj", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-catch-duplicates")]
    public Task early_catch_duplicates()
        => CompilationFailureTest("early-catch-duplicates", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-catch-function")]
    public Task early_catch_function()
        => CompilationFailureTest("early-catch-function", "Failed to parse JavaScript");

    [Fact(DisplayName = "early-catch-lex")]
    public Task early_catch_lex()
        => CompilationFailureTest("early-catch-lex", "Failed to parse JavaScript");

    [Fact(DisplayName = "optional-catch-binding-parens")]
    public Task optional_catch_binding_parens()
        => CompilationFailureTest("optional-catch-binding-parens", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.14_A16_T1")]
    public Task S12_14_A16_T1()
        => CompilationFailureTest("S12.14_A16_T1", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.14_A16_T10")]
    public Task S12_14_A16_T10()
        => CompilationFailureTest("S12.14_A16_T10", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.14_A16_T11")]
    public Task S12_14_A16_T11()
        => CompilationFailureTest("S12.14_A16_T11", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.14_A16_T12")]
    public Task S12_14_A16_T12()
        => CompilationFailureTest("S12.14_A16_T12", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.14_A16_T13")]
    public Task S12_14_A16_T13()
        => CompilationFailureTest("S12.14_A16_T13", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.14_A16_T14")]
    public Task S12_14_A16_T14()
        => CompilationFailureTest("S12.14_A16_T14", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.14_A16_T15")]
    public Task S12_14_A16_T15()
        => CompilationFailureTest("S12.14_A16_T15", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.14_A16_T2")]
    public Task S12_14_A16_T2()
        => CompilationFailureTest("S12.14_A16_T2", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.14_A16_T3")]
    public Task S12_14_A16_T3()
        => CompilationFailureTest("S12.14_A16_T3", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.14_A16_T5")]
    public Task S12_14_A16_T5()
        => CompilationFailureTest("S12.14_A16_T5", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.14_A16_T6")]
    public Task S12_14_A16_T6()
        => CompilationFailureTest("S12.14_A16_T6", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.14_A16_T7")]
    public Task S12_14_A16_T7()
        => CompilationFailureTest("S12.14_A16_T7", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.14_A16_T8")]
    public Task S12_14_A16_T8()
        => CompilationFailureTest("S12.14_A16_T8", "Failed to parse JavaScript");

    [Fact(DisplayName = "S12.14_A16_T9")]
    public Task S12_14_A16_T9()
        => CompilationFailureTest("S12.14_A16_T9", "Failed to parse JavaScript");

    [Fact(DisplayName = "static-init-await-binding-invalid")]
    public Task static_init_await_binding_invalid()
        => CompilationFailureTest("static-init-await-binding-invalid", "Failed to parse JavaScript");

}
