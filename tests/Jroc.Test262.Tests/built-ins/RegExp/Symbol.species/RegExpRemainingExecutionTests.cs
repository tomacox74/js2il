namespace Jroc.Test262.Tests.built_ins.RegExp.Symbol_species;

public sealed class RegExpRemainingExecutionTests : InMemoryExecutionTestsBase
{
    public RegExpRemainingExecutionTests() : base("built_ins.RegExp.Symbol_species") { }

    [Fact(DisplayName = "return-value.js")]
    public Task return_value()
        => ExecutionTest("return-value");

    [Fact(DisplayName = "symbol-species.js")]
    public Task symbol_species()
        => ExecutionTest("symbol-species");
}
