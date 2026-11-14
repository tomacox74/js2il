using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainTime.prototype.round;

public sealed class NativePortBatch_Port1000 : InMemoryExecutionTestsBase
{
    public NativePortBatch_Port1000() : base("built_ins.Temporal.PlainTime.prototype.round") { }

    [Fact(DisplayName = "branding.js")]
    public Task Port1000_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "options-read-before-algorithmic-validation.js")]
    public Task Port1000_options_read_before_algorithmic_validation() => ExecutionTestFromFile("options-read-before-algorithmic-validation");

    [Fact(DisplayName = "options-wrong-type.js")]
    public Task Port1000_options_wrong_type() => ExecutionTestFromFile("options-wrong-type");

    [Fact(DisplayName = "rounding-cross-midnight.js")]
    public Task Port1000_rounding_cross_midnight() => ExecutionTestFromFile("rounding-cross-midnight");

    [Fact(DisplayName = "roundingincrement-divides.js")]
    public Task Port1000_roundingincrement_divides() => ExecutionTestFromFile("roundingincrement-divides");

    [Fact(DisplayName = "roundingincrement-hours.js")]
    public Task Port1000_roundingincrement_hours() => ExecutionTestFromFile("roundingincrement-hours");

    [Fact(DisplayName = "roundingincrement-invalid.js")]
    public Task Port1000_roundingincrement_invalid() => ExecutionTestFromFile("roundingincrement-invalid");

    [Fact(DisplayName = "roundingincrement-microseconds.js")]
    public Task Port1000_roundingincrement_microseconds() => ExecutionTestFromFile("roundingincrement-microseconds");

    [Fact(DisplayName = "roundingincrement-milliseconds.js")]
    public Task Port1000_roundingincrement_milliseconds() => ExecutionTestFromFile("roundingincrement-milliseconds");

    [Fact(DisplayName = "roundingincrement-minutes.js")]
    public Task Port1000_roundingincrement_minutes() => ExecutionTestFromFile("roundingincrement-minutes");

    [Fact(DisplayName = "roundingincrement-nan.js")]
    public Task Port1000_roundingincrement_nan() => ExecutionTestFromFile("roundingincrement-nan");

    [Fact(DisplayName = "roundingincrement-nanoseconds.js")]
    public Task Port1000_roundingincrement_nanoseconds() => ExecutionTestFromFile("roundingincrement-nanoseconds");

    [Fact(DisplayName = "roundingincrement-non-integer.js")]
    public Task Port1000_roundingincrement_non_integer() => ExecutionTestFromFile("roundingincrement-non-integer");

    [Fact(DisplayName = "roundingincrement-out-of-range.js")]
    public Task Port1000_roundingincrement_out_of_range() => ExecutionTestFromFile("roundingincrement-out-of-range");

    [Fact(DisplayName = "roundingincrement-seconds.js")]
    public Task Port1000_roundingincrement_seconds() => ExecutionTestFromFile("roundingincrement-seconds");

    [Fact(DisplayName = "roundingincrement-undefined.js")]
    public Task Port1000_roundingincrement_undefined() => ExecutionTestFromFile("roundingincrement-undefined");

    [Fact(DisplayName = "roundingincrement-wrong-type.js")]
    public Task Port1000_roundingincrement_wrong_type() => ExecutionTestFromFile("roundingincrement-wrong-type");

    [Fact(DisplayName = "roundingmode-ceil.js")]
    public Task Port1000_roundingmode_ceil() => ExecutionTestFromFile("roundingmode-ceil");

    [Fact(DisplayName = "roundingmode-expand.js")]
    public Task Port1000_roundingmode_expand() => ExecutionTestFromFile("roundingmode-expand");

    [Fact(DisplayName = "roundingmode-floor.js")]
    public Task Port1000_roundingmode_floor() => ExecutionTestFromFile("roundingmode-floor");

    [Fact(DisplayName = "roundingmode-halfCeil.js")]
    public Task Port1000_roundingmode_halfCeil() => ExecutionTestFromFile("roundingmode-halfCeil");

    [Fact(DisplayName = "roundingmode-halfEven.js")]
    public Task Port1000_roundingmode_halfEven() => ExecutionTestFromFile("roundingmode-halfEven");

    [Fact(DisplayName = "roundingmode-halfExpand.js")]
    public Task Port1000_roundingmode_halfExpand() => ExecutionTestFromFile("roundingmode-halfExpand");

    [Fact(DisplayName = "roundingmode-halfFloor.js")]
    public Task Port1000_roundingmode_halfFloor() => ExecutionTestFromFile("roundingmode-halfFloor");

    [Fact(DisplayName = "roundingmode-halfTrunc.js")]
    public Task Port1000_roundingmode_halfTrunc() => ExecutionTestFromFile("roundingmode-halfTrunc");

    [Fact(DisplayName = "roundingmode-invalid-string.js")]
    public Task Port1000_roundingmode_invalid_string() => ExecutionTestFromFile("roundingmode-invalid-string");

    [Fact(DisplayName = "roundingmode-trunc.js")]
    public Task Port1000_roundingmode_trunc() => ExecutionTestFromFile("roundingmode-trunc");

    [Fact(DisplayName = "roundingmode-undefined.js")]
    public Task Port1000_roundingmode_undefined() => ExecutionTestFromFile("roundingmode-undefined");

    [Fact(DisplayName = "roundingmode-wrong-type.js")]
    public Task Port1000_roundingmode_wrong_type() => ExecutionTestFromFile("roundingmode-wrong-type");

    [Fact(DisplayName = "roundto-invalid-string.js")]
    public Task Port1000_roundto_invalid_string() => ExecutionTestFromFile("roundto-invalid-string");

    [Fact(DisplayName = "smallestunit-invalid-string.js")]
    public Task Port1000_smallestunit_invalid_string() => ExecutionTestFromFile("smallestunit-invalid-string");

    [Fact(DisplayName = "smallestunit-missing.js")]
    public Task Port1000_smallestunit_missing() => ExecutionTestFromFile("smallestunit-missing");

    [Fact(DisplayName = "smallestunit-plurals-accepted.js")]
    public Task Port1000_smallestunit_plurals_accepted() => ExecutionTestFromFile("smallestunit-plurals-accepted");

    [Fact(DisplayName = "smallestunit-string-shorthand.js")]
    public Task Port1000_smallestunit_string_shorthand() => ExecutionTestFromFile("smallestunit-string-shorthand");

    [Fact(DisplayName = "smallestunit-wrong-type.js")]
    public Task Port1000_smallestunit_wrong_type() => ExecutionTestFromFile("smallestunit-wrong-type");

    [Fact(DisplayName = "string-shorthand-no-object-prototype-pollution.js")]
    public Task Port1000_string_shorthand_no_object_prototype_pollution() => ExecutionTestFromFile("string-shorthand-no-object-prototype-pollution");

}
