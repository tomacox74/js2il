using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.TypedArray.prototype.keys;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("TypedArray.prototype.keys") { }

        [Fact(DisplayName = "iter-prototype")]
    public Task iter_prototype()
        => ExecutionTestFromFile("iter-prototype");
}
