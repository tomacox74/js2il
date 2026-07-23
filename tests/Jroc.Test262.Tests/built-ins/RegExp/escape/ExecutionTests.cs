using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.escape;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.RegExp.escape") { }

    [Fact(DisplayName = "escaped-control-characters")]
    public Task escaped_control_characters()
        => ExecutionTestFromFile("escaped-control-characters");

    [Fact(DisplayName = "escaped-lineterminator")]
    public Task escaped_lineterminator()
        => ExecutionTestFromFile("escaped-lineterminator");

    [Fact(DisplayName = "escaped-solidus-character-mixed")]
    public Task escaped_solidus_character_mixed()
        => ExecutionTestFromFile("escaped-solidus-character-mixed");

    [Fact(DisplayName = "escaped-surrogates")]
    public Task escaped_surrogates()
        => ExecutionTestFromFile("escaped-surrogates");

    [Fact(DisplayName = "escaped-syntax-characters-mixed")]
    public Task escaped_syntax_characters_mixed()
        => ExecutionTestFromFile("escaped-syntax-characters-mixed");

    [Fact(DisplayName = "escaped-utf16encodecodepoint")]
    public Task escaped_utf16encodecodepoint()
        => ExecutionTestFromFile("escaped-utf16encodecodepoint");

    [Fact(DisplayName = "escaped-whitespace")]
    public Task escaped_whitespace()
        => ExecutionTestFromFile("escaped-whitespace");

    [Fact(DisplayName = "initial-char-escape")]
    public Task initial_char_escape()
        => ExecutionTestFromFile("initial-char-escape");

    [Fact(DisplayName = "non-string-inputs")]
    public Task non_string_inputs()
        => ExecutionTestFromFile("non-string-inputs");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");
}
