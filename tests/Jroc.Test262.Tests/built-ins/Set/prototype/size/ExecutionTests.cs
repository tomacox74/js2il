using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.size;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Set.prototype.size") { }

    [Fact(DisplayName = "size")]
    public Task size()
        => ExecutionTestFromFile("size");

    [Fact(DisplayName = "returns-count-of-present-values-by-insertion")]
    public Task returns_count_of_present_values_by_insertion()
        => ExecutionTestFromFile("returns-count-of-present-values-by-insertion");

    [Fact(DisplayName = "returns-count-of-present-values-by-iterable")]
    public Task returns_count_of_present_values_by_iterable()
        => ExecutionTestFromFile("returns-count-of-present-values-by-iterable");

    [Fact(DisplayName = "returns-count-of-present-values-before-after-add-delete")]
    public Task returns_count_of_present_values_before_after_add_delete()
        => ExecutionTestFromFile("returns-count-of-present-values-before-after-add-delete");

}
