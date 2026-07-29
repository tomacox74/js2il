using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.isSubsetOf;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Set.prototype.isSubsetOf") { }

    [Fact(DisplayName = "allows-set-like-object")]
    public Task allows_set_like_object()
        => ExecutionTestFromFile("allows-set-like-object");
}
