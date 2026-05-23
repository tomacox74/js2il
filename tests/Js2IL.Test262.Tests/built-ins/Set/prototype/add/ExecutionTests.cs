using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Set.prototype.add;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Set.prototype.add") { }

    [Fact(DisplayName = "preserves-insertion-order")]
    public Task preserves_insertion_order()
        => ExecutionTestFromFile("preserves-insertion-order");

    [Fact(DisplayName = "returns-this-when-ignoring-duplicate")]
    public Task returns_this_when_ignoring_duplicate()
        => ExecutionTestFromFile("returns-this-when-ignoring-duplicate");

    [Fact(DisplayName = "returns-this")]
    public Task returns_this()
        => ExecutionTestFromFile("returns-this");
}
