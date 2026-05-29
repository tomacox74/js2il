using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Date.prototype.getTime;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Date.prototype.getTime") { }

    [Fact(DisplayName = "this-value-valid-date")]
    public Task this_value_valid_date()
        => ExecutionTestFromFile("this-value-valid-date");
}
