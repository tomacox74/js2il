using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.values;

public class SetRuntimeSemanticsBatchExecutionTests : InMemoryExecutionTestsBase
{
    public SetRuntimeSemanticsBatchExecutionTests() : base("built_ins.Set.prototype.values") { }

    [Fact(DisplayName = "values-iteration-mutable.js")]
    public Task values_iteration_mutable()
        => ExecutionTestFromFile("values-iteration-mutable");
}
