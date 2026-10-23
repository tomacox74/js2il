namespace Jroc.Test262.Tests.language.expressions.division;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.division") { }

    [Fact(DisplayName = "S11.5.2_A1.js", Skip = "eval is not supported.")]
    public Task S11_5_2_A1()
        => ExecutionTest("S11.5.2_A1");

    [Fact(DisplayName = "no-magic-asi-from-block-eval.js", Skip = "eval is not supported.")]
    public Task no_magic_asi_from_block_eval()
        => ExecutionTest("no-magic-asi-from-block-eval");
}
