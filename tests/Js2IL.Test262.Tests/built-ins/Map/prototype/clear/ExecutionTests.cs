using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Map.prototype.clear;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Map.prototype.clear") { }

    [Fact(DisplayName = "returns-undefined")]
    public Task returns_undefined()
        => ExecutionTestFromFile("returns-undefined");
    [Fact(DisplayName = "clear-map")]
    public Task clear_map()
        => ExecutionTestFromFile("clear-map");

    [Fact(DisplayName = "context-is-not-map-object")]
    public Task context_is_not_map_object()
        => ExecutionTestFromFile("context-is-not-map-object");

    [Fact(DisplayName = "context-is-not-object")]
    public Task context_is_not_object()
        => ExecutionTestFromFile("context-is-not-object");

    [Fact(DisplayName = "context-is-set-object-throws")]
    public Task context_is_set_object_throws()
        => ExecutionTestFromFile("context-is-set-object-throws");

    [Fact(DisplayName = "context-is-weakmap-object-throws")]
    public Task context_is_weakmap_object_throws()
        => ExecutionTestFromFile("context-is-weakmap-object-throws");

    [Fact(DisplayName = "map-data-list-is-preserved")]
    public Task map_data_list_is_preserved()
        => ExecutionTestFromFile("map-data-list-is-preserved");

}
