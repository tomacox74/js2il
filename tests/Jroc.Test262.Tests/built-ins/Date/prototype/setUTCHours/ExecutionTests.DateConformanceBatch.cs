using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Date.prototype.setUTCHours;

public partial class ExecutionTests
{
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

    [Fact(DisplayName = "this-value-valid-date-hour.js")]
    public Task this_value_valid_date_hour() => ExecutionTestFromFile("this-value-valid-date-hour");

    [Fact(DisplayName = "this-value-valid-date-min.js")]
    public Task this_value_valid_date_min() => ExecutionTestFromFile("this-value-valid-date-min");

    [Fact(DisplayName = "this-value-valid-date-ms.js")]
    public Task this_value_valid_date_ms() => ExecutionTestFromFile("this-value-valid-date-ms");

    [Fact(DisplayName = "this-value-valid-date-sec.js")]
    public Task this_value_valid_date_sec() => ExecutionTestFromFile("this-value-valid-date-sec");

}
