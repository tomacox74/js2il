using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Symbol.keyFor;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Symbol.keyFor") { }

    [Fact(DisplayName = "arg-symbol-registry-hit")]
    public Task arg_symbol_registry_hit()
        => ExecutionTestFromFile("arg-symbol-registry-hit");

    [Fact(DisplayName = "arg-symbol-registry-miss")]
    public Task arg_symbol_registry_miss()
        => ExecutionTestFromFile("arg-symbol-registry-miss");
}
