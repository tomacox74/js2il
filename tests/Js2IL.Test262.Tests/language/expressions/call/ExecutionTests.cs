using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.expressions.call;

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
}
