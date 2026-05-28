using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Map.prototype.has;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Map.prototype.has") { }

    [Fact(DisplayName = "has")]
    public Task has()
        => ExecutionTestFromFile("has");

    [Fact(DisplayName = "normalizes-zero-key")]
    public Task normalizes_zero_key()
        => ExecutionTestFromFile("normalizes-zero-key");

    [Fact(DisplayName = "return-true-different-key-types")]
    public Task return_true_different_key_types()
        => ExecutionTestFromFile("return-true-different-key-types");

    [Fact(DisplayName = "return-false-different-key-types")]
    public Task return_false_different_key_types()
        => ExecutionTestFromFile("return-false-different-key-types");

    [Fact(DisplayName = "this-not-object-throw")]
    public Task this_not_object_throw()
        => ExecutionTestFromFile("this-not-object-throw");
}
