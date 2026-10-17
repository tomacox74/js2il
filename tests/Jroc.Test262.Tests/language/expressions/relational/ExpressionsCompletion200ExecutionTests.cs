namespace Jroc.Test262.Tests.language.expressions.relational;

public sealed class ExpressionsCompletion200ExecutionTests : DiskExecutionTestsBase
{
    public ExpressionsCompletion200ExecutionTests() : base("language.expressions.relational") { }

    [Fact(DisplayName = "S9.1_A1_T4.js")]
    public Task S9_1_A1_T4()
        => ExecutionTest("S9.1_A1_T4");
}
