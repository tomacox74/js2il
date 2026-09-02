using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set;

public class SetRuntimeSemanticsBatchExecutionTests : DiskExecutionTestsBase
{
    public SetRuntimeSemanticsBatchExecutionTests() : base("built_ins.Set") { }

    [Fact(DisplayName = "set-iterator-close-after-add-failure.js")]
    public Task set_iterator_close_after_add_failure()
        => ExecutionTestFromFile("set-iterator-close-after-add-failure");
}
