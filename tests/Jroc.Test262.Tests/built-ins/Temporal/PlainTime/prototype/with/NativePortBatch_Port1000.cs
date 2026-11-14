using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Temporal.PlainTime.prototype.with;

public sealed class NativePortBatch_Port1000 : InMemoryExecutionTestsBase
{
    public NativePortBatch_Port1000() : base("built_ins.Temporal.PlainTime.prototype.with") { }

    [Fact(DisplayName = "argument-not-object.js")]
    public Task Port1000_argument_not_object() => ExecutionTestFromFile("argument-not-object");

    [Fact(DisplayName = "basic.js")]
    public Task Port1000_basic() => ExecutionTestFromFile("basic");

    [Fact(DisplayName = "branding.js")]
    public Task Port1000_branding() => ExecutionTestFromFile("branding");

    [Fact(DisplayName = "builtin.js")]
    public Task Port1000_builtin() => ExecutionTestFromFile("builtin");

    [Fact(DisplayName = "copy-properties-not-undefined.js")]
    public Task Port1000_copy_properties_not_undefined() => ExecutionTestFromFile("copy-properties-not-undefined");

    [Fact(DisplayName = "infinity-throws-rangeerror.js")]
    public Task Port1000_infinity_throws_rangeerror() => ExecutionTestFromFile("infinity-throws-rangeerror");

    [Fact(DisplayName = "length.js")]
    public Task Port1000_length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task Port1000_name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task Port1000_not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "options-invalid.js")]
    public Task Port1000_options_invalid() => ExecutionTestFromFile("options-invalid");

    [Fact(DisplayName = "options-object.js")]
    public Task Port1000_options_object() => ExecutionTestFromFile("options-object");

    [Fact(DisplayName = "options-read-before-algorithmic-validation.js")]
    public Task Port1000_options_read_before_algorithmic_validation() => ExecutionTestFromFile("options-read-before-algorithmic-validation");

    [Fact(DisplayName = "options-undefined.js")]
    public Task Port1000_options_undefined() => ExecutionTestFromFile("options-undefined");

    [Fact(DisplayName = "options-wrong-type.js")]
    public Task Port1000_options_wrong_type() => ExecutionTestFromFile("options-wrong-type");

    [Fact(DisplayName = "order-of-operations.js")]
    public Task Port1000_order_of_operations() => ExecutionTestFromFile("order-of-operations");

    [Fact(DisplayName = "overflow-invalid-string.js")]
    public Task Port1000_overflow_invalid_string() => ExecutionTestFromFile("overflow-invalid-string");

    [Fact(DisplayName = "overflow-undefined.js")]
    public Task Port1000_overflow_undefined() => ExecutionTestFromFile("overflow-undefined");

    [Fact(DisplayName = "overflow-wrong-type.js")]
    public Task Port1000_overflow_wrong_type() => ExecutionTestFromFile("overflow-wrong-type");

    [Fact(DisplayName = "prop-desc.js")]
    public Task Port1000_prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "throws-if-time-is-invalid-when-overflow-is-reject.js")]
    public Task Port1000_throws_if_time_is_invalid_when_overflow_is_reject() => ExecutionTestFromFile("throws-if-time-is-invalid-when-overflow-is-reject");

}
