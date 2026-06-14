using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.has;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Set.prototype.has") { }

    [Fact(DisplayName = "has")]
    public Task has()
        => ExecutionTestFromFile("has");

    [Fact(DisplayName = "returns-false-when-value-not-present-number")]
    public Task returns_false_when_value_not_present_number()
        => ExecutionTestFromFile("returns-false-when-value-not-present-number");

    [Fact(DisplayName = "returns-false-when-value-not-present-boolean")]
    public Task returns_false_when_value_not_present_boolean()
        => ExecutionTestFromFile("returns-false-when-value-not-present-boolean");

    [Fact(DisplayName = "returns-false-when-value-not-present-null")]
    public Task returns_false_when_value_not_present_null()
        => ExecutionTestFromFile("returns-false-when-value-not-present-null");

    [Fact(DisplayName = "returns-true-when-value-present-number")]
    public Task returns_true_when_value_present_number()
        => ExecutionTestFromFile("returns-true-when-value-present-number");

    [Fact(DisplayName = "returns-true-when-value-present-nan")]
    public Task returns_true_when_value_present_nan()
        => ExecutionTestFromFile("returns-true-when-value-present-nan");

}
