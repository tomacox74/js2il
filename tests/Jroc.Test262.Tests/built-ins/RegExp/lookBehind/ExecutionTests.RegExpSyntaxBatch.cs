using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.RegExp.lookBehind;

public partial class ExecutionTests
{
    [Fact(DisplayName = "alternations.js")]
    public Task alternations() => ExecutionTestFromFile("alternations");

    [Fact(DisplayName = "back-references.js")]
    public Task back_references() => ExecutionTestFromFile("back-references");

    [Fact(DisplayName = "captures-negative.js")]
    public Task captures_negative() => ExecutionTestFromFile("captures-negative");

    [Fact(DisplayName = "captures.js")]
    public Task captures() => ExecutionTestFromFile("captures");

    [Fact(DisplayName = "do-not-backtrack.js")]
    public Task do_not_backtrack() => ExecutionTestFromFile("do-not-backtrack");

    [Fact(DisplayName = "greedy-loop.js")]
    public Task greedy_loop() => ExecutionTestFromFile("greedy-loop");

    [Fact(DisplayName = "negative.js")]
    public Task negative() => ExecutionTestFromFile("negative");

    [Fact(DisplayName = "nested-lookaround.js")]
    public Task nested_lookaround() => ExecutionTestFromFile("nested-lookaround");

    [Fact(DisplayName = "simple-fixed-length.js")]
    public Task simple_fixed_length() => ExecutionTestFromFile("simple-fixed-length");

    [Fact(DisplayName = "sliced-strings.js")]
    public Task sliced_strings() => ExecutionTestFromFile("sliced-strings");

    [Fact(DisplayName = "start-of-line.js")]
    public Task start_of_line() => ExecutionTestFromFile("start-of-line");

    [Fact(DisplayName = "sticky.js")]
    public Task sticky() => ExecutionTestFromFile("sticky");

    [Fact(DisplayName = "variable-length.js")]
    public Task variable_length() => ExecutionTestFromFile("variable-length");

    [Fact(DisplayName = "word-boundary.js")]
    public Task word_boundary() => ExecutionTestFromFile("word-boundary");

}
