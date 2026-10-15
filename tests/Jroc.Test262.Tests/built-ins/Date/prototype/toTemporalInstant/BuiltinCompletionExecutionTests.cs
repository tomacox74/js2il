using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.prototype.toTemporalInstant;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.Date.prototype.toTemporalInstant") { }

    [Fact(DisplayName = "length.js")]
    public Task test_length() => ExecutionTest("length");

    [Fact(DisplayName = "name.js")]
    public Task test_name() => ExecutionTest("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task test_not_a_constructor() => ExecutionTest("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task test_prop_desc() => ExecutionTest("prop-desc");

    [Fact(DisplayName = "this-value-invalid-date.js")]
    public Task test_this_value_invalid_date() => ExecutionTest("this-value-invalid-date");

    [Fact(DisplayName = "this-value-non-date.js")]
    public Task test_this_value_non_date() => ExecutionTest("this-value-non-date");

    [Fact(DisplayName = "this-value-non-object.js")]
    public Task test_this_value_non_object() => ExecutionTest("this-value-non-object");

    [Fact(DisplayName = "this-value-valid-date.js")]
    public Task test_this_value_valid_date() => ExecutionTest("this-value-valid-date");
}
