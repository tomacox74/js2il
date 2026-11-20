using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainDate;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.PlainDate") { }

    [Fact(DisplayName = "argument-convert.js")]
    public Task Port300Nov19_argument_convert() => ExecutionTestFromFile("argument-convert");

    [Fact(DisplayName = "argument-invalid.js")]
    public Task Port300Nov19_argument_invalid() => ExecutionTestFromFile("argument-invalid");

    [Fact(DisplayName = "basic.js")]
    public Task Port300Nov19_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "builtin.js")]
    public Task Port300Nov19_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "calendar-case-insensitive.js")]
    public Task Port300Nov19_calendar_case_insensitive() => ExecutionTestFromFile("calendar-case-insensitive");

    [Fact(DisplayName = "calendar-invalid-iso-string.js")]
    public Task Port300Nov19_calendar_invalid_iso_string() => ExecutionTestFromFile("calendar-invalid-iso-string");

    [Fact(DisplayName = "calendar-string.js")]
    public Task Port300Nov19_calendar_string() => ExecutionTestFromFile("calendar-string");

    [Fact(DisplayName = "calendar-undefined.js")]
    public Task Port300Nov19_calendar_undefined() => ExecutionTestFromFile("calendar-undefined");

    [Fact(DisplayName = "calendar-wrong-type.js")]
    public Task Port300Nov19_calendar_wrong_type() => ExecutionTestFromFile("calendar-wrong-type");

    [Fact(DisplayName = "constructor.js")]
    public Task Port300Nov19_constructor() => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "get-prototype-from-constructor-throws.js")]
    public Task Port300Nov19_get_prototype_from_constructor_throws() => ExecutionTestFromFile("get-prototype-from-constructor-throws");

    [Fact(DisplayName = "infinity-throws-rangeerror.js")]
    public Task Port300Nov19_infinity_throws_rangeerror() => ExecutionTestFromFile("infinity-throws-rangeerror");

    [Fact(DisplayName = "length.js")]
    public Task Port300Nov19_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "limits.js")]
    public Task Port300Nov19_limits() => ExecutionTestFromFile("limits");

    [Fact(DisplayName = "missing-arguments.js")]
    public Task Port300Nov19_missing_arguments() => ExecutionTestFromFile("missing-arguments");

    [Fact(DisplayName = "name.js")]
    public Task Port300Nov19_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "negative-infinity-throws-rangeerror.js")]
    public Task Port300Nov19_negative_infinity_throws_rangeerror() => ExecutionTestFromFile("negative-infinity-throws-rangeerror");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300Nov19_prop_desc() => ExecutionTestFromFile("prop-desc");
}
