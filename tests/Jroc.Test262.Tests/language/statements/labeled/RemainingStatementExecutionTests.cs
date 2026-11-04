namespace Jroc.Test262.Tests.language.statements.labeled;

public sealed class RemainingStatementExecutionTests : DiskExecutionTestsBase
{
    public RemainingStatementExecutionTests() : base("Jroc.Test262.Tests.language.statements.labeled") { }

    [Fact(DisplayName = "let-block-with-newline.js")]
    public Task test_let_block_with_newline()
        => ExecutionTest("let-block-with-newline");

    [Fact(DisplayName = "value-await-module-escaped.js")]
    public Task test_value_await_module_escaped()
        => CompilationFailureTest("value-await-module-escaped", "Failed to parse JavaScript");

    [Fact(DisplayName = "value-await-module.js")]
    public Task test_value_await_module()
        => CompilationFailureTest("value-await-module", "Failed to parse JavaScript");

}
