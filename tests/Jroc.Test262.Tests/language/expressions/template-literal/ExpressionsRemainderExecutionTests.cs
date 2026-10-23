namespace Jroc.Test262.Tests.language.expressions.template_literal;

public sealed class ExpressionsRemainderExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsRemainderExecutionTests() : base("language.expressions.template_literal") { }

    [Fact(DisplayName = "invalid-unicode-escape-sequence-4.js")]
    public Task invalid_unicode_escape_sequence_4()
        => CompilationFailureTest("invalid-unicode-escape-sequence-4", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-unicode-escape-sequence-5.js")]
    public Task invalid_unicode_escape_sequence_5()
        => CompilationFailureTest("invalid-unicode-escape-sequence-5", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-unicode-escape-sequence-6.js")]
    public Task invalid_unicode_escape_sequence_6()
        => CompilationFailureTest("invalid-unicode-escape-sequence-6", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-unicode-escape-sequence-7.js")]
    public Task invalid_unicode_escape_sequence_7()
        => CompilationFailureTest("invalid-unicode-escape-sequence-7", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-unicode-escape-sequence-8.js")]
    public Task invalid_unicode_escape_sequence_8()
        => CompilationFailureTest("invalid-unicode-escape-sequence-8", "Failed to parse JavaScript");

    [Fact(DisplayName = "mongolian-vowel-separator-eval.js", Skip = "eval is not supported.")]
    public Task mongolian_vowel_separator_eval()
        => ExecutionTest("mongolian-vowel-separator-eval");

    [Fact(DisplayName = "unicode-escape-nls-err.js")]
    public Task unicode_escape_nls_err()
        => CompilationFailureTest("unicode-escape-nls-err", "Failed to parse JavaScript");

    [Fact(DisplayName = "unicode-escape-no-hex-err.js")]
    public Task unicode_escape_no_hex_err()
        => CompilationFailureTest("unicode-escape-no-hex-err", "Failed to parse JavaScript");
}
