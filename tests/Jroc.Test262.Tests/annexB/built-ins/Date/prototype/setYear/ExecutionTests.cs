using Xunit;

namespace Jroc.Test262.Tests.annexB.built_ins.Date.prototype.setYear;

public class ExecutionTests : Jroc.Test262.Tests.DiskExecutionTestsBase
{
    public ExecutionTests() : base("annexB/built-ins/Date/prototype/setYear") { }

    [Fact(DisplayName = "B.2.5.js")]
    public Task B_2_5() => ExecutionTest("B.2.5");

    [Fact(DisplayName = "date-value-read-before-tonumber-when-date-is-invalid.js")]
    public Task date_value_read_before_tonumber_when_date_is_invalid() => ExecutionTest("date-value-read-before-tonumber-when-date-is-invalid");

    [Fact(DisplayName = "date-value-read-before-tonumber-when-date-is-valid.js")]
    public Task date_value_read_before_tonumber_when_date_is_valid() => ExecutionTest("date-value-read-before-tonumber-when-date-is-valid");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTest("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTest("name");

    [Fact(DisplayName = "this-not-date.js")]
    public Task this_not_date() => ExecutionTest("this-not-date");

    [Fact(DisplayName = "this-time-nan.js")]
    public Task this_time_nan() => ExecutionTest("this-time-nan");

    [Fact(DisplayName = "this-time-valid.js")]
    public Task this_time_valid() => ExecutionTest("this-time-valid");

    [Fact(DisplayName = "time-clip.js")]
    public Task time_clip() => ExecutionTest("time-clip");

    [Fact(DisplayName = "year-nan.js")]
    public Task year_nan() => ExecutionTest("year-nan");

    [Fact(DisplayName = "year-number-absolute.js")]
    public Task year_number_absolute() => ExecutionTest("year-number-absolute");

    [Fact(DisplayName = "year-number-relative.js")]
    public Task year_number_relative() => ExecutionTest("year-number-relative");

    [Fact(DisplayName = "year-to-number-err.js")]
    public Task year_to_number_err() => ExecutionTest("year-to-number-err");
}
