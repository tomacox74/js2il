using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.flat;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.flat") { }

    [Fact(DisplayName = "empty-object-elements")]
    public Task empty_object_elements()
        => ExecutionTestFromFile("empty-object-elements");

    [Fact(DisplayName = "null-undefined-input-throws")]
    public Task null_undefined_input_throws()
        => ExecutionTestFromFile("null-undefined-input-throws");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "null-undefined-elements")]
    public Task null_undefined_elements()
        => ExecutionTestFromFile("null-undefined-elements");

    [Fact(DisplayName = "empty-array-elements")]
    public Task empty_array_elements()
        => ExecutionTestFromFile("empty-array-elements");

    [Fact(DisplayName = "non-numeric-depth-should-not-throw")]
    public Task non_numeric_depth_should_not_throw()
        => ExecutionTestFromFile("non-numeric-depth-should-not-throw");

    [Fact(DisplayName = "positive-infinity")]
    public Task positive_infinity()
        => ExecutionTestFromFile("positive-infinity");

}
