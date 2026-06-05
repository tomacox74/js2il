using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.WeakMap.prototype.set;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.WeakMap.prototype.set") { }

    [Fact(DisplayName = "set")]
    public Task set()
        => ExecutionTestFromFile("set");

    [Fact(DisplayName = "returns-this")]
    public Task returns_this()
        => ExecutionTestFromFile("returns-this");

    [Fact(DisplayName = "adds-object-element")]
    public Task adds_object_element()
        => ExecutionTestFromFile("adds-object-element");

    [Fact(DisplayName = "returns-this-when-ignoring-duplicate")]
    public Task returns_this_when_ignoring_duplicate()
        => ExecutionTestFromFile("returns-this-when-ignoring-duplicate");

    [Fact(DisplayName = "does-not-have-weakmapdata-internal-slot-array")]
    public Task does_not_have_weakmapdata_internal_slot_array()
        => ExecutionTestFromFile("does-not-have-weakmapdata-internal-slot-array");

    [Fact(DisplayName = "this-not-object-throw-null")]
    public Task this_not_object_throw_null()
        => ExecutionTestFromFile("this-not-object-throw-null");

    [Fact(DisplayName = "this-not-object-throw-undefined")]
    public Task this_not_object_throw_undefined()
        => ExecutionTestFromFile("this-not-object-throw-undefined");

}
