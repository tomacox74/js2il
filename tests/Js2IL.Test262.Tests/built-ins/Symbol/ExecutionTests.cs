using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Symbol;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Symbol") { }

    [Fact(DisplayName = "auto-boxing-non-strict", Skip = "Tracked by #1093: JS2IL does not yet pass this advanced test262 scenario.")]
    public Task auto_boxing_non_strict()
        => ExecutionTestFromFile("auto-boxing-non-strict");

    [Fact(DisplayName = "constructor", Skip = "Tracked by #1093: JS2IL does not yet pass this advanced test262 scenario.")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");
}
