using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainTime;

public sealed class NativePortBatch_Port1000 : InMemoryExecutionTestsBase
{
    public NativePortBatch_Port1000() : base("built_ins.Temporal.PlainTime") { }

    [Fact(DisplayName = "argument-convert.js")]
    public Task Port1000_argument_convert() => ExecutionTestFromFile("argument-convert");

    [Fact(DisplayName = "basic.js")]
    public Task Port1000_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "builtin.js")]
    public Task Port1000_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "constructor.js")]
    public Task Port1000_constructor() => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "get-prototype-from-constructor-throws.js")]
    public Task Port1000_get_prototype_from_constructor_throws() => ExecutionTestFromFile("get-prototype-from-constructor-throws");

    [Fact(DisplayName = "hour-undefined.js")]
    public Task Port1000_hour_undefined() => ExecutionTestFromFile("hour-undefined");

    [Fact(DisplayName = "infinity-throws-rangeerror.js")]
    public Task Port1000_infinity_throws_rangeerror() => ExecutionTestFromFile("infinity-throws-rangeerror");

    [Fact(DisplayName = "length.js")]
    public Task Port1000_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "microsecond-undefined.js")]
    public Task Port1000_microsecond_undefined() => ExecutionTestFromFile("microsecond-undefined");

    [Fact(DisplayName = "millisecond-undefined.js")]
    public Task Port1000_millisecond_undefined() => ExecutionTestFromFile("millisecond-undefined");

    [Fact(DisplayName = "minute-undefined.js")]
    public Task Port1000_minute_undefined() => ExecutionTestFromFile("minute-undefined");

    [Fact(DisplayName = "name.js")]
    public Task Port1000_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "nanosecond-undefined.js")]
    public Task Port1000_nanosecond_undefined() => ExecutionTestFromFile("nanosecond-undefined");

    [Fact(DisplayName = "negative-infinity-throws-rangeerror.js")]
    public Task Port1000_negative_infinity_throws_rangeerror() => ExecutionTestFromFile("negative-infinity-throws-rangeerror");

    [Fact(DisplayName = "negative-zero.js")]
    public Task Port1000_negative_zero() => ExecutionTestFromFile("negative-zero");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port1000_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "second-undefined.js")]
    public Task Port1000_second_undefined() => ExecutionTestFromFile("second-undefined");

    [Fact(DisplayName = "subclass.js")]
    public Task Port1000_subclass() => ExecutionTestFromFile("subclass");

    [Fact(DisplayName = "throws-if-time-is-invalid.js")]
    public Task Port1000_throws_if_time_is_invalid() => ExecutionTestFromFile("throws-if-time-is-invalid");

}
