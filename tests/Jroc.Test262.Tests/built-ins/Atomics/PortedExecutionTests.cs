using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Atomics;

public sealed class PortedExecutionTests : ExecutionTestsBase
{
    public PortedExecutionTests() : base("built_ins.Atomics") { }

    [Fact(DisplayName = "Symbol.toStringTag")]
    public Task Symbol_toStringTag()
        => ExecutionTest("Symbol.toStringTag");

}
