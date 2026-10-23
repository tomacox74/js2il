namespace Jroc.Test262.Tests.built_ins.RegExp.prototype.source;

public sealed class RegExpRemainingExecutionTests : InMemoryExecutionTestsBase
{
    public RegExpRemainingExecutionTests() : base("built_ins.RegExp.prototype.source") { }

    [Fact(DisplayName = "cross-realm.js")]
    public Task cross_realm()
        => ExecutionTest("cross-realm");

    [Fact(DisplayName = "this-val-regexp-prototype.js")]
    public Task this_val_regexp_prototype()
        => ExecutionTest("this-val-regexp-prototype");

    [Fact(DisplayName = "value-empty.js", Skip = "eval is not supported.")]
    public Task value_empty()
        => ExecutionTest("value-empty");

    [Fact(DisplayName = "value-line-terminator.js", Skip = "eval is not supported.")]
    public Task value_line_terminator()
        => ExecutionTest("value-line-terminator");

    [Fact(DisplayName = "value-slash.js", Skip = "eval is not supported.")]
    public Task value_slash()
        => ExecutionTest("value-slash");

    [Fact(DisplayName = "value-u.js", Skip = "eval is not supported.")]
    public Task value_u()
        => ExecutionTest("value-u");

    [Fact(DisplayName = "value.js", Skip = "eval is not supported.")]
    public Task value()
        => ExecutionTest("value");
}
