using Xunit;

namespace Jroc.Test262.Tests.language.comments.hashbang;

public sealed class RemainingCommentsExecutionTests : Jroc.Test262.Tests.DiskExecutionTestsBase
{
    public RemainingCommentsExecutionTests() : base("language.comments.hashbang")
    {
    }

    [Fact(DisplayName = "language/comments/hashbang/escaped-bang-041.js")]
    public Task test_escaped_bang_041() => CompilationFailureTest("escaped-bang-041", expectedFailureText: "Failed to parse JavaScript");

    [Fact(DisplayName = "language/comments/hashbang/escaped-bang-u0021.js")]
    public Task test_escaped_bang_u0021() => CompilationFailureTest("escaped-bang-u0021", expectedFailureText: "Failed to parse JavaScript");

    [Fact(DisplayName = "language/comments/hashbang/escaped-bang-u21.js")]
    public Task test_escaped_bang_u21() => CompilationFailureTest("escaped-bang-u21", expectedFailureText: "Failed to parse JavaScript");

    [Fact(DisplayName = "language/comments/hashbang/escaped-bang-x21.js")]
    public Task test_escaped_bang_x21() => CompilationFailureTest("escaped-bang-x21", expectedFailureText: "Failed to parse JavaScript");

    [Fact(DisplayName = "language/comments/hashbang/escaped-hash-043.js")]
    public Task test_escaped_hash_043() => CompilationFailureTest("escaped-hash-043", expectedFailureText: "Failed to parse JavaScript");

    [Fact(DisplayName = "language/comments/hashbang/escaped-hash-u0023.js")]
    public Task test_escaped_hash_u0023() => CompilationFailureTest("escaped-hash-u0023", expectedFailureText: "Failed to parse JavaScript");

    [Fact(DisplayName = "language/comments/hashbang/escaped-hash-u23.js")]
    public Task test_escaped_hash_u23() => CompilationFailureTest("escaped-hash-u23", expectedFailureText: "Failed to parse JavaScript");

    [Fact(DisplayName = "language/comments/hashbang/escaped-hash-x23.js")]
    public Task test_escaped_hash_x23() => CompilationFailureTest("escaped-hash-x23", expectedFailureText: "Failed to parse JavaScript");

    [Fact(DisplayName = "language/comments/hashbang/escaped-hashbang.js")]
    public Task test_escaped_hashbang() => CompilationFailureTest("escaped-hashbang", expectedFailureText: "Failed to parse JavaScript");

    [Fact(DisplayName = "language/comments/hashbang/eval-indirect.js", Skip = "Blocked: eval is not supported yet.")]
    public Task test_eval_indirect() => ExecutionTest("eval-indirect");

    [Fact(DisplayName = "language/comments/hashbang/eval.js", Skip = "Blocked: eval is not supported yet.")]
    public Task test_eval() => ExecutionTest("eval");

    [Fact(DisplayName = "language/comments/hashbang/function-constructor.js")]
    public Task test_function_constructor() => ExecutionTest("function-constructor");

    [Fact(DisplayName = "language/comments/hashbang/line-terminator-carriage-return.js")]
    public Task test_line_terminator_carriage_return() => ExecutionTest("line-terminator-carriage-return");

    [Fact(DisplayName = "language/comments/hashbang/line-terminator-line-separator.js")]
    public Task test_line_terminator_line_separator() => ExecutionTest("line-terminator-line-separator");

    [Fact(DisplayName = "language/comments/hashbang/line-terminator-paragraph-separator.js")]
    public Task test_line_terminator_paragraph_separator() => ExecutionTest("line-terminator-paragraph-separator");

    [Fact(DisplayName = "language/comments/hashbang/module.js")]
    public Task test_module() => ExecutionTest("module");

    [Fact(DisplayName = "language/comments/hashbang/multi-line-comment.js")]
    public Task test_multi_line_comment() => CompilationFailureTest("multi-line-comment", expectedFailureText: "Failed to parse JavaScript");

    [Fact(DisplayName = "language/comments/hashbang/no-line-separator.js", Skip = "Blocked: eval is not supported yet.")]
    public Task test_no_line_separator() => ExecutionTest("no-line-separator");

    [Fact(DisplayName = "language/comments/hashbang/not-empty.js")]
    public Task test_not_empty() => ExecutionTest("not-empty");

    [Fact(DisplayName = "language/comments/hashbang/preceding-directive-prologue-sc.js")]
    public Task test_preceding_directive_prologue_sc() => CompilationFailureTest("preceding-directive-prologue-sc", expectedFailureText: "Failed to parse JavaScript");

    [Fact(DisplayName = "language/comments/hashbang/preceding-directive-prologue.js")]
    public Task test_preceding_directive_prologue() => CompilationFailureTest("preceding-directive-prologue", expectedFailureText: "Failed to parse JavaScript");

    [Fact(DisplayName = "language/comments/hashbang/preceding-empty-statement.js")]
    public Task test_preceding_empty_statement() => CompilationFailureTest("preceding-empty-statement", expectedFailureText: "Failed to parse JavaScript");

    [Fact(DisplayName = "language/comments/hashbang/preceding-hashbang.js")]
    public Task test_preceding_hashbang() => CompilationFailureTest("preceding-hashbang", expectedFailureText: "Failed to parse JavaScript");

    [Fact(DisplayName = "language/comments/hashbang/preceding-line-comment.js")]
    public Task test_preceding_line_comment() => CompilationFailureTest("preceding-line-comment", expectedFailureText: "Failed to parse JavaScript");

    [Fact(DisplayName = "language/comments/hashbang/preceding-multi-line-comment.js")]
    public Task test_preceding_multi_line_comment() => CompilationFailureTest("preceding-multi-line-comment", expectedFailureText: "Failed to parse JavaScript");

    [Fact(DisplayName = "language/comments/hashbang/preceding-whitespace.js")]
    public Task test_preceding_whitespace() => CompilationFailureTest("preceding-whitespace", expectedFailureText: "Failed to parse JavaScript");

    [Fact(DisplayName = "language/comments/hashbang/use-strict.js")]
    public Task test_use_strict() => ExecutionTest("use-strict");
}
