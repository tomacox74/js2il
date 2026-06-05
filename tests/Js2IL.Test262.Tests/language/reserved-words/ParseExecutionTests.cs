using Js2IL.Test262.Tests.language.modules;

namespace Js2IL.Test262.Tests.language.reserved_words;

public class ParseExecutionTests : Js2IL.Test262.Tests.language.modules.FileSystemExecutionTestsBase
{
    public ParseExecutionTests() : base(@"language\reserved-words", "language.reserved_words") { }

    [Fact(DisplayName = "ident-reference-false")]
    public Task ident_reference_false()
        => CompilationFailureTest("ident-reference-false");

    [Fact(DisplayName = "ident-reference-false-escaped")]
    public Task ident_reference_false_escaped()
        => CompilationFailureTest("ident-reference-false-escaped");

    [Fact(DisplayName = "ident-reference-null")]
    public Task ident_reference_null()
        => CompilationFailureTest("ident-reference-null");

    [Fact(DisplayName = "ident-reference-null-escaped")]
    public Task ident_reference_null_escaped()
        => CompilationFailureTest("ident-reference-null-escaped");

    [Fact(DisplayName = "ident-reference-true")]
    public Task ident_reference_true()
        => CompilationFailureTest("ident-reference-true");

    [Fact(DisplayName = "ident-reference-true-escaped")]
    public Task ident_reference_true_escaped()
        => CompilationFailureTest("ident-reference-true-escaped");

    [Fact(DisplayName = "label-ident-false")]
    public Task label_ident_false()
        => CompilationFailureTest("label-ident-false");

    [Fact(DisplayName = "label-ident-false-escaped")]
    public Task label_ident_false_escaped()
        => CompilationFailureTest("label-ident-false-escaped");

    [Fact(DisplayName = "label-ident-null")]
    public Task label_ident_null()
        => CompilationFailureTest("label-ident-null");

    [Fact(DisplayName = "label-ident-null-escaped")]
    public Task label_ident_null_escaped()
        => CompilationFailureTest("label-ident-null-escaped");

    [Fact(DisplayName = "label-ident-true")]
    public Task label_ident_true()
        => CompilationFailureTest("label-ident-true");

    [Fact(DisplayName = "label-ident-true-escaped")]
    public Task label_ident_true_escaped()
        => CompilationFailureTest("label-ident-true-escaped");

}
