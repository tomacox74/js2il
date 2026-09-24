namespace Jroc.Test262.Tests.built_ins.Date.prototype.setHours;

public partial class ExecutionTests
{
    [Fact(DisplayName = "arg-coercion-order.js")]
    public Task arg_coercion_order() => ExecutionTestFromFile("arg-coercion-order");

    [Fact(DisplayName = "arg-hour-to-number.js")]
    public Task arg_hour_to_number() => ExecutionTestFromFile("arg-hour-to-number");

    [Fact(DisplayName = "arg-min-to-number.js")]
    public Task arg_min_to_number() => ExecutionTestFromFile("arg-min-to-number");

    [Fact(DisplayName = "arg-ms-to-number.js")]
    public Task arg_ms_to_number() => ExecutionTestFromFile("arg-ms-to-number");

    [Fact(DisplayName = "arg-sec-to-number.js")]
    public Task arg_sec_to_number() => ExecutionTestFromFile("arg-sec-to-number");

    [Fact(DisplayName = "date-value-read-before-tonumber-when-date-is-invalid.js")]
    public Task date_value_read_before_tonumber_when_date_is_invalid() => ExecutionTestFromFile("date-value-read-before-tonumber-when-date-is-invalid");

    [Fact(DisplayName = "new-value-time-clip.js")]
    public Task new_value_time_clip() => ExecutionTestFromFile("new-value-time-clip");

}
