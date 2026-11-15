using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.Duration;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.Duration") { }

    [Fact(DisplayName = "basic.js")]
    public Task Port300Nov14_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "builtin.js")]
    public Task Port300Nov14_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "call-builtin.js")]
    public Task Port300Nov14_call_builtin() => ExecutionTestFromFile("call-builtin");

    [Fact(DisplayName = "constructor.js")]
    public Task Port300Nov14_constructor() => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "days-undefined.js")]
    public Task Port300Nov14_days_undefined() => ExecutionTestFromFile("days-undefined");

    [Fact(DisplayName = "fractional-throws-rangeerror.js")]
    public Task Port300Nov14_fractional_throws_rangeerror() => ExecutionTestFromFile("fractional-throws-rangeerror");

    [Fact(DisplayName = "get-prototype-from-constructor-throws.js")]
    public Task Port300Nov14_get_prototype_from_constructor_throws() => ExecutionTestFromFile("get-prototype-from-constructor-throws");

    [Fact(DisplayName = "hours-undefined.js")]
    public Task Port300Nov14_hours_undefined() => ExecutionTestFromFile("hours-undefined");

    [Fact(DisplayName = "infinity-throws-rangeerror.js")]
    public Task Port300Nov14_infinity_throws_rangeerror() => ExecutionTestFromFile("infinity-throws-rangeerror");

    [Fact(DisplayName = "invalid-type.js")]
    public Task Port300Nov14_invalid_type() => ExecutionTestFromFile("invalid-type");

    [Fact(DisplayName = "large-number.js")]
    public Task Port300Nov14_large_number() => ExecutionTestFromFile("large-number");

    [Fact(DisplayName = "length.js")]
    public Task Port300Nov14_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "lower-limit.js")]
    public Task Port300Nov14_lower_limit() => ExecutionTestFromFile("lower-limit");

    [Fact(DisplayName = "max.js")]
    public Task Port300Nov14_max() => ExecutionTestFromFile("max");

    [Fact(DisplayName = "microseconds-undefined.js")]
    public Task Port300Nov14_microseconds_undefined() => ExecutionTestFromFile("microseconds-undefined");

    [Fact(DisplayName = "milliseconds-undefined.js")]
    public Task Port300Nov14_milliseconds_undefined() => ExecutionTestFromFile("milliseconds-undefined");

    [Fact(DisplayName = "minutes-undefined.js")]
    public Task Port300Nov14_minutes_undefined() => ExecutionTestFromFile("minutes-undefined");

    [Fact(DisplayName = "mixed.js")]
    public Task Port300Nov14_mixed() => ExecutionTestFromFile("mixed");

    [Fact(DisplayName = "months-undefined.js")]
    public Task Port300Nov14_months_undefined() => ExecutionTestFromFile("months-undefined");

    [Fact(DisplayName = "name.js")]
    public Task Port300Nov14_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "nanoseconds-undefined.js")]
    public Task Port300Nov14_nanoseconds_undefined() => ExecutionTestFromFile("nanoseconds-undefined");

    [Fact(DisplayName = "negative-infinity-throws-rangeerror.js")]
    public Task Port300Nov14_negative_infinity_throws_rangeerror() => ExecutionTestFromFile("negative-infinity-throws-rangeerror");

    [Fact(DisplayName = "out-of-range.js")]
    public Task Port300Nov14_out_of_range() => ExecutionTestFromFile("out-of-range");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300Nov14_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "seconds-undefined.js")]
    public Task Port300Nov14_seconds_undefined() => ExecutionTestFromFile("seconds-undefined");

    [Fact(DisplayName = "subclass.js")]
    public Task Port300Nov14_subclass() => ExecutionTestFromFile("subclass");

    [Fact(DisplayName = "weeks-undefined.js")]
    public Task Port300Nov14_weeks_undefined() => ExecutionTestFromFile("weeks-undefined");

    [Fact(DisplayName = "years-undefined.js")]
    public Task Port300Nov14_years_undefined() => ExecutionTestFromFile("years-undefined");

}
