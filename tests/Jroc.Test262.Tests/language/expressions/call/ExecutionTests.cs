using Jroc.Tests;

namespace Jroc.Test262.Tests.language.expressions.call;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.call") { }

    [Fact(DisplayName = "scope-lex-close")]
    public Task scope_lex_close()
        => ExecutionTest("scope-lex-close", preferOutOfProc: true);

    [Fact(DisplayName = "scope-lex-open")]
    public Task scope_lex_open()
        => ExecutionTest("scope-lex-open", preferOutOfProc: true);

    [Fact(DisplayName = "scope-var-close")]
    public Task scope_var_close()
        => ExecutionTest("scope-var-close");

    [Fact(DisplayName = "scope-var-open")]
    public Task scope_var_open()
        => ExecutionTest("scope-var-open");

    [Fact(DisplayName = "spread-sngl-empty")]
    public Task spread_sngl_empty()
        => ExecutionTest("spread-sngl-empty");

    [Fact(DisplayName = "spread-sngl-expr")]
    public Task spread_sngl_expr()
        => ExecutionTest("spread-sngl-expr");

    [Fact(DisplayName = "spread-sngl-iter")]
    public Task spread_sngl_iter()
        => ExecutionTest("spread-sngl-iter");

    [Fact(DisplayName = "spread-mult-empty")]
    public Task spread_mult_empty()
        => ExecutionTest("spread-mult-empty");

    [Fact(DisplayName = "spread-mult-expr")]
    public Task spread_mult_expr()
        => ExecutionTest("spread-mult-expr");

    [Fact(DisplayName = "spread-mult-literal")]
    public Task spread_mult_literal()
        => ExecutionTest("spread-mult-literal");

    [Fact(DisplayName = "spread-mult-iter")]
    public Task spread_mult_iter()
        => ExecutionTest("spread-mult-iter");
}
