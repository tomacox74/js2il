using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakRef.prototype.deref;

public class Test262BatchPort20260920Round5ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260920Round5ExecutionTests() : base("built_ins.WeakRef.prototype.deref") { }

    [Fact(DisplayName = "return-object-target")]
    public Task return_object_target()
        => ExecutionTestFromFile("return-object-target");

    [Fact(DisplayName = "return-symbol-target")]
    public Task return_symbol_target()
        => ExecutionTestFromFile("return-symbol-target");

}
