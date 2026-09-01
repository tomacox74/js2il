using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.prototype.setMonth;

public partial class ExecutionTests
{
    [Fact(DisplayName = "arg-date-to-number-err.js")]
    public Task arg_date_to_number_err() => ExecutionTestFromFile("arg-date-to-number-err");

    [Fact(DisplayName = "arg-month-to-number-err.js")]
    public Task arg_month_to_number_err() => ExecutionTestFromFile("arg-month-to-number-err");

    [Fact(DisplayName = "date-value-read-before-tonumber-when-date-is-valid.js")]
    public Task date_value_read_before_tonumber_when_date_is_valid() => ExecutionTestFromFile("date-value-read-before-tonumber-when-date-is-valid");

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

    [Fact(DisplayName = "this-value-valid-date-date.js")]
    public Task this_value_valid_date_date() => ExecutionTestFromFile("this-value-valid-date-date");

    [Fact(DisplayName = "this-value-valid-date-month.js")]
    public Task this_value_valid_date_month() => ExecutionTestFromFile("this-value-valid-date-month");

}
