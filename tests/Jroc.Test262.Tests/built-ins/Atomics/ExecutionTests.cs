using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Atomics;

public sealed class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Atomics") { }

    [Fact(DisplayName = "Symbol.toStringTag")]
    public Task Symbol_toStringTag()
        => ExecutionTest("Symbol.toStringTag");

}
