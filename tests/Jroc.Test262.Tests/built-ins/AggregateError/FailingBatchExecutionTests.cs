using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AggregateError;

public class PromiseBatchExecutionTests : DiskExecutionTestsBase
{
    public PromiseBatchExecutionTests() : base("built_ins.AggregateError") { }

    [Fact(DisplayName = "cause-property.js")]
    public Task cause_property()
        => ExecutionTestFromFile("cause-property");

}
