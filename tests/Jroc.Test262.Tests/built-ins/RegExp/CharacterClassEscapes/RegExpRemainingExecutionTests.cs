namespace Jroc.Test262.Tests.built_ins.RegExp.CharacterClassEscapes;

public sealed class RegExpRemainingExecutionTests : InMemoryExecutionTestsBase
{
    public RegExpRemainingExecutionTests() : base("built_ins.RegExp.CharacterClassEscapes") { }

    [Fact(DisplayName = "character-class-digit-class-escape-negative-cases.js")]
    public Task character_class_digit_class_escape_negative_cases()
        => ExecutionTest("character-class-digit-class-escape-negative-cases");

    [Fact(DisplayName = "character-class-digit-class-escape-positive-cases.js")]
    public Task character_class_digit_class_escape_positive_cases()
        => ExecutionTest("character-class-digit-class-escape-positive-cases");

    [Fact(DisplayName = "character-class-non-digit-class-escape-negative-cases.js")]
    public Task character_class_non_digit_class_escape_negative_cases()
        => ExecutionTest("character-class-non-digit-class-escape-negative-cases");

    [Fact(DisplayName = "character-class-non-digit-class-escape-positive-cases.js")]
    public Task character_class_non_digit_class_escape_positive_cases()
        => ExecutionTest("character-class-non-digit-class-escape-positive-cases");

    [Fact(DisplayName = "character-class-non-whitespace-class-escape-negative-cases.js")]
    public Task character_class_non_whitespace_class_escape_negative_cases()
        => ExecutionTest("character-class-non-whitespace-class-escape-negative-cases");

    [Fact(DisplayName = "character-class-non-whitespace-class-escape-positive-cases.js")]
    public Task character_class_non_whitespace_class_escape_positive_cases()
        => ExecutionTest("character-class-non-whitespace-class-escape-positive-cases");

    [Fact(DisplayName = "character-class-non-word-class-escape-negative-cases.js")]
    public Task character_class_non_word_class_escape_negative_cases()
        => ExecutionTest("character-class-non-word-class-escape-negative-cases");

    [Fact(DisplayName = "character-class-non-word-class-escape-positive-cases.js")]
    public Task character_class_non_word_class_escape_positive_cases()
        => ExecutionTest("character-class-non-word-class-escape-positive-cases");

    [Fact(DisplayName = "character-class-whitespace-class-escape-negative-cases.js")]
    public Task character_class_whitespace_class_escape_negative_cases()
        => ExecutionTest("character-class-whitespace-class-escape-negative-cases");

    [Fact(DisplayName = "character-class-whitespace-class-escape-positive-cases.js")]
    public Task character_class_whitespace_class_escape_positive_cases()
        => ExecutionTest("character-class-whitespace-class-escape-positive-cases");

    [Fact(DisplayName = "character-class-word-class-escape-negative-cases.js")]
    public Task character_class_word_class_escape_negative_cases()
        => ExecutionTest("character-class-word-class-escape-negative-cases");

    [Fact(DisplayName = "character-class-word-class-escape-positive-cases.js")]
    public Task character_class_word_class_escape_positive_cases()
        => ExecutionTest("character-class-word-class-escape-positive-cases");
}
