using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.prototype.toISOString;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Date.prototype.toISOString") { }

    [Fact(DisplayName = "15.9.5.43-0-16")]
    public Task _15_9_5_43_0_16()
        => ExecutionTestFromFile("15.9.5.43-0-16");

    [Fact(DisplayName = "15.9.5.43-0-5")]
    public Task _15_9_5_43_0_5()
        => ExecutionTestFromFile("15.9.5.43-0-5");

    [Fact(DisplayName = "15.9.5.43-0-6")]
    public Task _15_9_5_43_0_6()
        => ExecutionTestFromFile("15.9.5.43-0-6");

    [Fact(DisplayName = "15.9.5.43-0-7")]
    public Task _15_9_5_43_0_7()
        => ExecutionTestFromFile("15.9.5.43-0-7");
}
