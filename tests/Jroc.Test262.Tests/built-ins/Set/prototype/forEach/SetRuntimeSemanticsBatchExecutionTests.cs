using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.forEach;

public class SetRuntimeSemanticsBatchExecutionTests : DiskExecutionTestsBase
{
    public SetRuntimeSemanticsBatchExecutionTests() : base("built_ins.Set.prototype.forEach") { }

    [Fact(DisplayName = "iterates-values-revisits-after-delete-re-add.js")]
    public Task iterates_values_revisits_after_delete_re_add()
        => ExecutionTestFromFile("iterates-values-revisits-after-delete-re-add");

    [Fact(DisplayName = "length.js")]
    public Task length()
        => ExecutionTestFromFile("length");
}
