using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.fromCodePoint;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.fromCodePoint") { }

    [Fact(DisplayName = "argument-is-Symbol.js")]
    public Task argument_is_Symbol()
        => ExecutionTestFromFile("argument-is-Symbol");

    [Fact(DisplayName = "argument-not-coercible.js")]
    public Task argument_not_coercible()
        => ExecutionTestFromFile("argument-not-coercible");
}
