using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Function.prototype.Symbol.hasInstance;

public class FunctionConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public FunctionConformanceBatchExecutionTests() : base("built_ins.Function.prototype.Symbol.hasInstance") { }

    [Fact(DisplayName = "this-val-prototype-non-obj.js")]
    public Task this_val_prototype_non_obj() => ExecutionTestFromFile("this-val-prototype-non-obj");

}
