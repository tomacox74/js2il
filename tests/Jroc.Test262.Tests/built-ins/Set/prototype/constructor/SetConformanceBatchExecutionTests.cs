using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.constructor;

public class SetConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public SetConformanceBatchExecutionTests() : base("built_ins.Set.prototype.constructor") { }

    [Fact(DisplayName = "set-prototype-constructor-intrinsic.js")]
    public Task set_prototype_constructor_intrinsic() => ExecutionTestFromFile("set-prototype-constructor-intrinsic");

    [Fact(DisplayName = "set-prototype-constructor.js")]
    public Task set_prototype_constructor() => ExecutionTestFromFile("set-prototype-constructor");

}
