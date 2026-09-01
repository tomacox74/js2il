using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Function.prototype.caller_arguments;

public class FunctionConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public FunctionConformanceBatchExecutionTests() : base("built_ins.Function.prototype.caller-arguments") { }

    [Fact(DisplayName = "accessor-properties.js")]
    public Task accessor_properties() => ExecutionTestFromFile("accessor-properties");

}
