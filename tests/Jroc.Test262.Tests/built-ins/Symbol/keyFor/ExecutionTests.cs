using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Symbol.keyFor;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Symbol.keyFor") { }

    [Fact(DisplayName = "arg-non-symbol")]
    public Task arg_non_symbol()
        => ExecutionTestFromFile("arg-non-symbol");

    [Fact(DisplayName = "arg-symbol-registry-hit")]
    public Task arg_symbol_registry_hit()
        => ExecutionTestFromFile("arg-symbol-registry-hit");

    [Fact(DisplayName = "arg-symbol-registry-miss")]
    public Task arg_symbol_registry_miss()
        => ExecutionTestFromFile("arg-symbol-registry-miss");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");
}
