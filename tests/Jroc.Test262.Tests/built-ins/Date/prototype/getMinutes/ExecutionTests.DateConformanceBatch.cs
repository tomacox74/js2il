using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.prototype.getMinutes;

public partial class ExecutionTests
{
    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "this-value-invalid-date.js")]
    public Task this_value_invalid_date() => ExecutionTestFromFile("this-value-invalid-date");

    [Fact(DisplayName = "this-value-non-date.js")]
    public Task this_value_non_date() => ExecutionTestFromFile("this-value-non-date");

    [Fact(DisplayName = "this-value-non-object.js")]
    public Task this_value_non_object() => ExecutionTestFromFile("this-value-non-object");

    [Fact(DisplayName = "this-value-valid-date.js")]
    public Task this_value_valid_date() => ExecutionTestFromFile("this-value-valid-date");

}
