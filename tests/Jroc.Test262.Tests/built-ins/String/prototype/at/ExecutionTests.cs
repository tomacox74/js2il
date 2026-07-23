using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.at;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.String.prototype.at") { }

    [Fact(DisplayName = "length")] public Task length() => ExecutionTestFromFile("length");
    [Fact(DisplayName = "name")] public Task name() => ExecutionTestFromFile("name");
    [Fact(DisplayName = "returns-item")] public Task returns_item() => ExecutionTestFromFile("returns-item");
    [Fact(DisplayName = "returns-item-relative-index")] public Task returns_item_relative_index() => ExecutionTestFromFile("returns-item-relative-index");
}
