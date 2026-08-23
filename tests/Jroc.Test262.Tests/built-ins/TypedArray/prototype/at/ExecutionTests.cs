using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.at;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.TypedArray.prototype.at") { }

    [Fact(DisplayName = "returns-item-relative-index")]
    public Task returns_item_relative_index()
        => ExecutionTestFromFile("returns-item-relative-index");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "index-argument-tointeger")]
    public Task index_argument_tointeger()
        => ExecutionTestFromFile("index-argument-tointeger");

    [Fact(DisplayName = "returns-item")]
    public Task returns_item()
        => ExecutionTestFromFile("returns-item");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "return-abrupt-from-this")]
    public Task return_abrupt_from_this()
        => ExecutionTestFromFile("return-abrupt-from-this");

    [Fact(DisplayName = "index-non-numeric-argument-tointeger-invalid")]
    public Task index_non_numeric_argument_tointeger_invalid()
        => ExecutionTestFromFile("index-non-numeric-argument-tointeger-invalid");
}
