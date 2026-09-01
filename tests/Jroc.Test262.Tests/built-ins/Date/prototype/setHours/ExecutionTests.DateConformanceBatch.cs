using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.prototype.setHours;

public partial class ExecutionTests
{
    [Fact(DisplayName = "arg-hour-to-number-err.js")]
    public Task arg_hour_to_number_err() => ExecutionTestFromFile("arg-hour-to-number-err");

    [Fact(DisplayName = "arg-min-to-number-err.js")]
    public Task arg_min_to_number_err() => ExecutionTestFromFile("arg-min-to-number-err");

    [Fact(DisplayName = "arg-ms-to-number-err.js")]
    public Task arg_ms_to_number_err() => ExecutionTestFromFile("arg-ms-to-number-err");

    [Fact(DisplayName = "arg-sec-to-number-err.js")]
    public Task arg_sec_to_number_err() => ExecutionTestFromFile("arg-sec-to-number-err");

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

    [Fact(DisplayName = "this-value-valid-date-hour.js")]
    public Task this_value_valid_date_hour() => ExecutionTestFromFile("this-value-valid-date-hour");

    [Fact(DisplayName = "this-value-valid-date-min.js")]
    public Task this_value_valid_date_min() => ExecutionTestFromFile("this-value-valid-date-min");

    [Fact(DisplayName = "this-value-valid-date-ms.js")]
    public Task this_value_valid_date_ms() => ExecutionTestFromFile("this-value-valid-date-ms");

    [Fact(DisplayName = "this-value-valid-date-sec.js")]
    public Task this_value_valid_date_sec() => ExecutionTestFromFile("this-value-valid-date-sec");

}
