using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.statements.@for;

public class Test262BatchPort20260919Round4ExecutionTests : DiskExecutionTestsBase
{
    public Test262BatchPort20260919Round4ExecutionTests() : base("language.statements.for") { }

    [Fact(DisplayName = "S12.6.3_A10.1_T1")]
    public Task S12_6_3_A10_1_T1()
        => ExecutionTest("S12.6.3_A10.1_T1");

    [Fact(DisplayName = "S12.6.3_A10.1_T2")]
    public Task S12_6_3_A10_1_T2()
        => ExecutionTest("S12.6.3_A10.1_T2");

    [Fact(DisplayName = "head-init-async-of")]
    public Task head_init_async_of()
        => ExecutionTest("head-init-async-of");

    [Fact(DisplayName = "head-let-destructuring")]
    public Task head_let_destructuring()
        => ExecutionTest("head-let-destructuring");

    [Fact(DisplayName = "head-lhs-let")]
    public Task head_lhs_let()
        => ExecutionTest("head-lhs-let");

    [Fact(DisplayName = "let-block-with-newline")]
    public Task let_block_with_newline()
        => ExecutionTest("let-block-with-newline");

    [Fact(DisplayName = "let-identifier-with-newline")]
    public Task let_identifier_with_newline()
        => ExecutionTest("let-identifier-with-newline");

}
