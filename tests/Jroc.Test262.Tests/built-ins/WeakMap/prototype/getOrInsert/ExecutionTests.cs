using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakMap.prototype.getOrInsert;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.WeakMap.prototype.getOrInsert") { }

    [Fact(DisplayName = "adds-object-element")]
    public Task adds_object_element()
        => ExecutionTestFromFile("adds-object-element");

    [Fact(DisplayName = "getOrInsert")]
    public Task getOrInsert()
        => ExecutionTestFromFile("getOrInsert");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "returns-value-if-key-is-not-present-object-key")]
    public Task returns_value_if_key_is_not_present_object_key()
        => ExecutionTestFromFile("returns-value-if-key-is-not-present-object-key");

    [Fact(DisplayName = "returns-value-if-key-is-present-object-key")]
    public Task returns_value_if_key_is_present_object_key()
        => ExecutionTestFromFile("returns-value-if-key-is-present-object-key");
}
