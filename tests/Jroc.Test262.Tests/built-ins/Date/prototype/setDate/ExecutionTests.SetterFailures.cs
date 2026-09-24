namespace Jroc.Test262.Tests.built_ins.Date.prototype.setDate;

public partial class ExecutionTests
{
    [Fact(DisplayName = "arg-coercion-order.js")]
    public Task arg_coercion_order() => ExecutionTestFromFile("arg-coercion-order");

    [Fact(DisplayName = "arg-to-number.js")]
    public Task arg_to_number() => ExecutionTestFromFile("arg-to-number");

    [Fact(DisplayName = "date-value-read-before-tonumber-when-date-is-invalid.js")]
    public Task date_value_read_before_tonumber_when_date_is_invalid() => ExecutionTestFromFile("date-value-read-before-tonumber-when-date-is-invalid");

    [Fact(DisplayName = "new-value-time-clip.js")]
    public Task new_value_time_clip() => ExecutionTestFromFile("new-value-time-clip");

}
