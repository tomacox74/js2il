using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Symbol;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Symbol") { }

    [Fact(DisplayName = "auto-boxing-non-strict")]
    public Task auto_boxing_non_strict()
        => ExecutionTestFromFile("auto-boxing-non-strict");

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "not-callable")]
    public Task not_callable()
        => ExecutionTestFromFile("not-callable");

    [Fact(DisplayName = "symbol")]
    public Task symbol()
        => ExecutionTestFromFile("symbol");

    [Fact(DisplayName = "uniqueness")]
    public Task uniqueness()
        => ExecutionTestFromFile("uniqueness");
}
