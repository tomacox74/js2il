using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.JSON;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.JSON") { }

    [Fact(DisplayName = "15.12-0-1")]
    public Task _15_12_0_1()
        => ExecutionTest("15.12-0-1");

    [Fact(DisplayName = "15.12-0-4")]
    public Task _15_12_0_4()
        => ExecutionTest("15.12-0-4", preferOutOfProc: true);

    [Fact(DisplayName = "Symbol.toStringTag")]
    public Task Symbol_toStringTag()
        => ExecutionTest("Symbol.toStringTag");
}
