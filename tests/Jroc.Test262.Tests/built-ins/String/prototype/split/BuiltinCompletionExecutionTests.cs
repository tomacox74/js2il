using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.split;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.String.prototype.split") { }

    [Fact(DisplayName = "checking-by-using-eval.js", Skip = "eval is not supported.")]
    public Task test_checking_by_using_eval() => ExecutionTest("checking-by-using-eval");

    [Fact(DisplayName = "separator-regexp-limit-string-via-eval.js", Skip = "eval is not supported.")]
    public Task test_separator_regexp_limit_string_via_eval() => ExecutionTest("separator-regexp-limit-string-via-eval");
}
