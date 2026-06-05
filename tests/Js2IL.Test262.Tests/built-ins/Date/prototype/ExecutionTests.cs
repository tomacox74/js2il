using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Date.prototype;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Date.prototype") { }

    [Fact(DisplayName = "S15.9.5_A01_T1")]
    public Task S15_9_5_A01_T1()
        => ExecutionTestFromFile("S15.9.5_A01_T1");

    [Fact(DisplayName = "no-date-value")]
    public Task no_date_value()
        => ExecutionTestFromFile("no-date-value");
}
