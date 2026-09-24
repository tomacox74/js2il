namespace Jroc.Test262.Tests.built_ins.Date.prototype.setFullYear;

public partial class ExecutionTests
{
    [Fact(DisplayName = "arg-date-to-number.js")]
    public Task arg_date_to_number() => ExecutionTestFromFile("arg-date-to-number");

    [Fact(DisplayName = "arg-month-to-number.js")]
    public Task arg_month_to_number() => ExecutionTestFromFile("arg-month-to-number");

    [Fact(DisplayName = "arg-year-to-number.js")]
    public Task arg_year_to_number() => ExecutionTestFromFile("arg-year-to-number");

    [Fact(DisplayName = "date-value-read-before-tonumber-when-date-is-invalid.js")]
    public Task date_value_read_before_tonumber_when_date_is_invalid() => ExecutionTestFromFile("date-value-read-before-tonumber-when-date-is-invalid");

    [Fact(DisplayName = "new-value-time-clip.js")]
    public Task new_value_time_clip() => ExecutionTestFromFile("new-value-time-clip");

    [Fact(DisplayName = "this-value-invalid-date.js")]
    public Task this_value_invalid_date() => ExecutionTestFromFile("this-value-invalid-date");

}
