using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.Instant.prototype.round;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.Instant.prototype.round") { }

    [Fact(DisplayName = "accepts-plural-units.js")]
    public Task Port300InstantNov15_accepts_plural_units() => ExecutionTestFromFile("accepts-plural-units");

    [Fact(DisplayName = "accepts-string-parameter-for-smallestunit.js")]
    public Task Port300InstantNov15_accepts_string_parameter_for_smallestunit() => ExecutionTestFromFile("accepts-string-parameter-for-smallestunit");

    [Fact(DisplayName = "allow-increments-that-divide-evenly-into-solar-days.js")]
    public Task Port300InstantNov15_allow_increments_that_divide_evenly_into_solar_days() => ExecutionTestFromFile("allow-increments-that-divide-evenly-into-solar-days");

    [Fact(DisplayName = "branding.js")]
    public Task Port300InstantNov15_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "builtin.js")]
    public Task Port300InstantNov15_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "length.js")]
    public Task Port300InstantNov15_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task Port300InstantNov15_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "negative-instant.js")]
    public Task Port300InstantNov15_negative_instant() => ExecutionTestFromFile("negative-instant");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port300InstantNov15_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "options-read-before-algorithmic-validation.js")]
    public Task Port300InstantNov15_options_read_before_algorithmic_validation() => ExecutionTestFromFile("options-read-before-algorithmic-validation");

    [Fact(DisplayName = "options-wrong-type.js")]
    public Task Port300InstantNov15_options_wrong_type() => ExecutionTestFromFile("options-wrong-type");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300InstantNov15_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "round-to-days.js")]
    public Task Port300InstantNov15_round_to_days() => ExecutionTestFromFile("round-to-days");

    [Fact(DisplayName = "rounding-direction.js")]
    public Task Port300InstantNov15_rounding_direction() => ExecutionTestFromFile("rounding-direction");

    [Fact(DisplayName = "rounding-increments.js")]
    public Task Port300InstantNov15_rounding_increments() => ExecutionTestFromFile("rounding-increments");

    [Fact(DisplayName = "roundingincrement-nan.js")]
    public Task Port300InstantNov15_roundingincrement_nan() => ExecutionTestFromFile("roundingincrement-nan");

    [Fact(DisplayName = "roundingincrement-non-integer.js")]
    public Task Port300InstantNov15_roundingincrement_non_integer() => ExecutionTestFromFile("roundingincrement-non-integer");

    [Fact(DisplayName = "roundingincrement-out-of-range.js")]
    public Task Port300InstantNov15_roundingincrement_out_of_range() => ExecutionTestFromFile("roundingincrement-out-of-range");

    [Fact(DisplayName = "roundingincrement-undefined.js")]
    public Task Port300InstantNov15_roundingincrement_undefined() => ExecutionTestFromFile("roundingincrement-undefined");

    [Fact(DisplayName = "roundingincrement-wrong-type.js")]
    public Task Port300InstantNov15_roundingincrement_wrong_type() => ExecutionTestFromFile("roundingincrement-wrong-type");

    [Fact(DisplayName = "roundingmode-ceil.js")]
    public Task Port300InstantNov15_roundingmode_ceil() => ExecutionTestFromFile("roundingmode-ceil");

    [Fact(DisplayName = "roundingmode-expand.js")]
    public Task Port300InstantNov15_roundingmode_expand() => ExecutionTestFromFile("roundingmode-expand");

    [Fact(DisplayName = "roundingmode-floor.js")]
    public Task Port300InstantNov15_roundingmode_floor() => ExecutionTestFromFile("roundingmode-floor");

    [Fact(DisplayName = "roundingmode-halfCeil.js")]
    public Task Port300InstantNov15_roundingmode_halfCeil() => ExecutionTestFromFile("roundingmode-halfCeil");

    [Fact(DisplayName = "roundingmode-halfEven.js")]
    public Task Port300InstantNov15_roundingmode_halfEven() => ExecutionTestFromFile("roundingmode-halfEven");

    [Fact(DisplayName = "roundingmode-halfExpand.js")]
    public Task Port300InstantNov15_roundingmode_halfExpand() => ExecutionTestFromFile("roundingmode-halfExpand");

    [Fact(DisplayName = "roundingmode-halfFloor.js")]
    public Task Port300InstantNov15_roundingmode_halfFloor() => ExecutionTestFromFile("roundingmode-halfFloor");

    [Fact(DisplayName = "roundingmode-halfTrunc.js")]
    public Task Port300InstantNov15_roundingmode_halfTrunc() => ExecutionTestFromFile("roundingmode-halfTrunc");

    [Fact(DisplayName = "roundingmode-invalid-string.js")]
    public Task Port300InstantNov15_roundingmode_invalid_string() => ExecutionTestFromFile("roundingmode-invalid-string");

    [Fact(DisplayName = "roundingmode-trunc.js")]
    public Task Port300InstantNov15_roundingmode_trunc() => ExecutionTestFromFile("roundingmode-trunc");

    [Fact(DisplayName = "roundingmode-undefined.js")]
    public Task Port300InstantNov15_roundingmode_undefined() => ExecutionTestFromFile("roundingmode-undefined");

    [Fact(DisplayName = "roundingmode-wrong-type.js")]
    public Task Port300InstantNov15_roundingmode_wrong_type() => ExecutionTestFromFile("roundingmode-wrong-type");

    [Fact(DisplayName = "roundto-invalid-string.js")]
    public Task Port300InstantNov15_roundto_invalid_string() => ExecutionTestFromFile("roundto-invalid-string");

    [Fact(DisplayName = "smallestunit-invalid-string.js")]
    public Task Port300InstantNov15_smallestunit_invalid_string() => ExecutionTestFromFile("smallestunit-invalid-string");

    [Fact(DisplayName = "smallestunit-plurals-accepted.js")]
    public Task Port300InstantNov15_smallestunit_plurals_accepted() => ExecutionTestFromFile("smallestunit-plurals-accepted");

    [Fact(DisplayName = "smallestunit-string-shorthand.js")]
    public Task Port300InstantNov15_smallestunit_string_shorthand() => ExecutionTestFromFile("smallestunit-string-shorthand");

    [Fact(DisplayName = "smallestunit-wrong-type.js")]
    public Task Port300InstantNov15_smallestunit_wrong_type() => ExecutionTestFromFile("smallestunit-wrong-type");

    [Fact(DisplayName = "string-shorthand-no-object-prototype-pollution.js")]
    public Task Port300InstantNov15_string_shorthand_no_object_prototype_pollution() => ExecutionTestFromFile("string-shorthand-no-object-prototype-pollution");

    [Fact(DisplayName = "subclassing-ignored.js")]
    public Task Port300InstantNov15_subclassing_ignored() => ExecutionTestFromFile("subclassing-ignored");

    [Fact(DisplayName = "throws-on-increments-that-do-not-divide-evenly.js")]
    public Task Port300InstantNov15_throws_on_increments_that_do_not_divide_evenly() => ExecutionTestFromFile("throws-on-increments-that-do-not-divide-evenly");

    [Fact(DisplayName = "throws-without-smallest-unit.js")]
    public Task Port300InstantNov15_throws_without_smallest_unit() => ExecutionTestFromFile("throws-without-smallest-unit");
}
