using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.Duration.prototype.with;

public sealed class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Temporal.Duration.prototype.with") { }

    [Fact(DisplayName = "all-negative.js")]
    public Task Port300Nov14_all_negative() => ExecutionTestFromFile("all-negative");

    [Fact(DisplayName = "all-positive.js")]
    public Task Port300Nov14_all_positive() => ExecutionTestFromFile("all-positive");

    [Fact(DisplayName = "argument-invalid-property.js")]
    public Task Port300Nov14_argument_invalid_property() => ExecutionTestFromFile("argument-invalid-property");

    [Fact(DisplayName = "argument-mixed-sign.js")]
    public Task Port300Nov14_argument_mixed_sign() => ExecutionTestFromFile("argument-mixed-sign");

    [Fact(DisplayName = "argument-not-object.js")]
    public Task Port300Nov14_argument_not_object() => ExecutionTestFromFile("argument-not-object");

    [Fact(DisplayName = "argument-singular-properties.js")]
    public Task Port300Nov14_argument_singular_properties() => ExecutionTestFromFile("argument-singular-properties");

    [Fact(DisplayName = "blank-duration.js")]
    public Task Port300Nov14_blank_duration() => ExecutionTestFromFile("blank-duration");

    [Fact(DisplayName = "branding.js")]
    public Task Port300Nov14_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "builtin.js")]
    public Task Port300Nov14_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "copy-properties-not-undefined.js")]
    public Task Port300Nov14_copy_properties_not_undefined() => ExecutionTestFromFile("copy-properties-not-undefined");

    [Fact(DisplayName = "infinity-throws-rangeerror.js")]
    public Task Port300Nov14_infinity_throws_rangeerror() => ExecutionTestFromFile("infinity-throws-rangeerror");

    [Fact(DisplayName = "length.js")]
    public Task Port300Nov14_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task Port300Nov14_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "negative-infinity-throws-rangeerror.js")]
    public Task Port300Nov14_negative_infinity_throws_rangeerror() => ExecutionTestFromFile("negative-infinity-throws-rangeerror");

    [Fact(DisplayName = "non-integer-throws-rangeerror.js")]
    public Task Port300Nov14_non_integer_throws_rangeerror() => ExecutionTestFromFile("non-integer-throws-rangeerror");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port300Nov14_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "order-of-operations.js")]
    public Task Port300Nov14_order_of_operations() => ExecutionTestFromFile("order-of-operations");

    [Fact(DisplayName = "partial-positive.js")]
    public Task Port300Nov14_partial_positive() => ExecutionTestFromFile("partial-positive");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port300Nov14_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "sign-conflict-throws-rangeerror.js")]
    public Task Port300Nov14_sign_conflict_throws_rangeerror() => ExecutionTestFromFile("sign-conflict-throws-rangeerror");

    [Fact(DisplayName = "sign-replace.js")]
    public Task Port300Nov14_sign_replace() => ExecutionTestFromFile("sign-replace");

    [Fact(DisplayName = "subclassing-ignored.js")]
    public Task Port300Nov14_subclassing_ignored() => ExecutionTestFromFile("subclassing-ignored");

}
