using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.escape;

public partial class ExecutionTests
{
    [Fact(DisplayName = "escaped-otherpunctuators.js")]
    public Task escaped_otherpunctuators() => ExecutionTestFromFile("escaped-otherpunctuators");

    [Fact(DisplayName = "escaped-solidus-character-simple.js")]
    public Task escaped_solidus_character_simple() => ExecutionTestFromFile("escaped-solidus-character-simple");

    [Fact(DisplayName = "escaped-syntax-characters-simple.js")]
    public Task escaped_syntax_characters_simple() => ExecutionTestFromFile("escaped-syntax-characters-simple");

    [Fact(DisplayName = "is-function.js")]
    public Task is_function() => ExecutionTestFromFile("is-function");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "not-escaped-underscore.js")]
    public Task not_escaped_underscore() => ExecutionTestFromFile("not-escaped-underscore");

    [Fact(DisplayName = "not-escaped.js")]
    public Task not_escaped() => ExecutionTestFromFile("not-escaped");

}
