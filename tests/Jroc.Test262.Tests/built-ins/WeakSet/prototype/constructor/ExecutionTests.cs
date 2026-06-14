using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakSet.prototype.constructor;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.WeakSet.prototype.constructor") { }

    [Fact(DisplayName = "weakset-prototype-constructor")]
    public Task weakset_prototype_constructor()
        => ExecutionTestFromFile("weakset-prototype-constructor");

}
