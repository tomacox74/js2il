namespace Jroc.Test262.Tests.language.expressions.template_literal;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.template_literal") { }

    [Fact(DisplayName = "invalid-hexidecimal-character-escape-sequence-truncated-1.js")]
    public Task invalid_hexidecimal_character_escape_sequence_truncated_1()
        => CompilationFailureTest("invalid-hexidecimal-character-escape-sequence-truncated-1", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-hexidecimal-character-escape-sequence-truncated-2.js")]
    public Task invalid_hexidecimal_character_escape_sequence_truncated_2()
        => CompilationFailureTest("invalid-hexidecimal-character-escape-sequence-truncated-2", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-hexidecimal-character-escape-sequence-truncated-3.js")]
    public Task invalid_hexidecimal_character_escape_sequence_truncated_3()
        => CompilationFailureTest("invalid-hexidecimal-character-escape-sequence-truncated-3", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-legacy-octal-escape-sequence-8.js")]
    public Task invalid_legacy_octal_escape_sequence_8()
        => CompilationFailureTest("invalid-legacy-octal-escape-sequence-8", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-legacy-octal-escape-sequence-9.js")]
    public Task invalid_legacy_octal_escape_sequence_9()
        => CompilationFailureTest("invalid-legacy-octal-escape-sequence-9", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-legacy-octal-escape-sequence.js")]
    public Task invalid_legacy_octal_escape_sequence()
        => CompilationFailureTest("invalid-legacy-octal-escape-sequence", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-unicode-escape-sequence-1.js")]
    public Task invalid_unicode_escape_sequence_1()
        => CompilationFailureTest("invalid-unicode-escape-sequence-1", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-unicode-escape-sequence-2.js")]
    public Task invalid_unicode_escape_sequence_2()
        => CompilationFailureTest("invalid-unicode-escape-sequence-2", "Failed to parse JavaScript");

    [Fact(DisplayName = "invalid-unicode-escape-sequence-3.js")]
    public Task invalid_unicode_escape_sequence_3()
        => CompilationFailureTest("invalid-unicode-escape-sequence-3", "Failed to parse JavaScript");
}
