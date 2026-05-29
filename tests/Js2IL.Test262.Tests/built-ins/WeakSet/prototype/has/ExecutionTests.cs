using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.WeakSet.prototype.has;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.WeakSet.prototype.has") { }

    [Fact(DisplayName = "returns-true-when-object-value-present")]
    public Task returns_true_when_object_value_present()
        => ExecutionTestFromFile("returns-true-when-object-value-present");

    [Fact(DisplayName = "has")]
    public Task has()
        => ExecutionTestFromFile("has");

    [Fact(DisplayName = "returns-false-when-object-value-not-present")]
    public Task returns_false_when_object_value_not_present()
        => ExecutionTestFromFile("returns-false-when-object-value-not-present");

    [Fact(DisplayName = "returns-false-when-value-cannot-be-held-weakly")]
    public Task returns_false_when_value_cannot_be_held_weakly()
        => ExecutionTestFromFile("returns-false-when-value-cannot-be-held-weakly");

    [Fact(DisplayName = "this-not-object-throw-undefined")]
    public Task this_not_object_throw_undefined()
        => ExecutionTestFromFile("this-not-object-throw-undefined");
}
