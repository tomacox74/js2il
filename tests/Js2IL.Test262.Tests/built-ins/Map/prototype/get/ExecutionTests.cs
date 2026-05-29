using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Map.prototype.get;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Map.prototype.get") { }

    [Fact(DisplayName = "get")]
    public Task get()
        => ExecutionTestFromFile("get");

    [Fact(DisplayName = "returns-undefined")]
    public Task returns_undefined()
        => ExecutionTestFromFile("returns-undefined");

    [Fact(DisplayName = "returns-value-normalized-zero-key")]
    public Task returns_value_normalized_zero_key()
        => ExecutionTestFromFile("returns-value-normalized-zero-key");

    [Fact(DisplayName = "returns-value-different-key-types")]
    public Task returns_value_different_key_types()
        => ExecutionTestFromFile("returns-value-different-key-types");

    [Fact(DisplayName = "this-not-object-throw")]
    public Task this_not_object_throw()
        => ExecutionTestFromFile("this-not-object-throw");
}
