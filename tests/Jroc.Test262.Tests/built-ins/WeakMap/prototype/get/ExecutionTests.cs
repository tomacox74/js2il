using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakMap.prototype.get;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.WeakMap.prototype.get") { }

    [Fact(DisplayName = "returns-value-with-object-key")]
    public Task returns_value_with_object_key()
        => ExecutionTestFromFile("returns-value-with-object-key");

    [Fact(DisplayName = "get")]
    public Task get()
        => ExecutionTestFromFile("get");

    [Fact(DisplayName = "returns-undefined-with-object-key")]
    public Task returns_undefined_with_object_key()
        => ExecutionTestFromFile("returns-undefined-with-object-key");

    [Fact(DisplayName = "returns-undefined-if-key-cannot-be-held-weakly")]
    public Task returns_undefined_if_key_cannot_be_held_weakly()
        => ExecutionTestFromFile("returns-undefined-if-key-cannot-be-held-weakly");

    [Fact(DisplayName = "this-not-object-throw")]
    public Task this_not_object_throw()
        => ExecutionTestFromFile("this-not-object-throw");
}
