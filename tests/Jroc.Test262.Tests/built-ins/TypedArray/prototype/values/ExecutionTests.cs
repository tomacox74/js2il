using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.values;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("TypedArray.prototype.values") { }

        [Fact(DisplayName = "iter-prototype")]
    public Task iter_prototype()
        => ExecutionTestFromFile("iter-prototype");
}
