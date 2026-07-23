using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.prototype.toString;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.prototype.toString") { }

    [Fact(DisplayName = "Object.prototype.toString.call-bigint")]
    public Task Object_prototype_toString_call_bigint()
        => ExecutionTestFromFile("Object.prototype.toString.call-bigint");

    [Fact(DisplayName = "symbol-tag-map-builtin")]
    public Task symbol_tag_map_builtin()
        => ExecutionTestFromFile("symbol-tag-map-builtin");

    [Fact(DisplayName = "symbol-tag-set-builtin")]
    public Task symbol_tag_set_builtin()
        => ExecutionTestFromFile("symbol-tag-set-builtin");

    [Fact(DisplayName = "symbol-tag-weakmap-builtin")]
    public Task symbol_tag_weakmap_builtin()
        => ExecutionTestFromFile("symbol-tag-weakmap-builtin");
}
