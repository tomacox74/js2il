namespace Jroc.Test262.Tests.built_ins.String.prototype.codePointAt;

public partial class ExecutionTests
{
    [Fact(DisplayName = "codePointAt.js")]
    public Task codePointAt() => ExecutionTestFromFile("codePointAt");
    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");
    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");
    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
    [Fact(DisplayName = "return-abrupt-from-object-pos-to-integer.js")]
    public Task return_abrupt_from_object_pos_to_integer() => ExecutionTestFromFile("return-abrupt-from-object-pos-to-integer");
    [Fact(DisplayName = "return-abrupt-from-symbol-pos-to-integer.js")]
    public Task return_abrupt_from_symbol_pos_to_integer() => ExecutionTestFromFile("return-abrupt-from-symbol-pos-to-integer");
    [Fact(DisplayName = "return-abrupt-from-this-as-symbol.js")]
    public Task return_abrupt_from_this_as_symbol() => ExecutionTestFromFile("return-abrupt-from-this-as-symbol");
    [Fact(DisplayName = "return-abrupt-from-this.js")]
    public Task return_abrupt_from_this() => ExecutionTestFromFile("return-abrupt-from-this");
    [Fact(DisplayName = "return-code-unit-coerced-position.js")]
    public Task return_code_unit_coerced_position() => ExecutionTestFromFile("return-code-unit-coerced-position");
    [Fact(DisplayName = "return-first-code-unit.js")]
    public Task return_first_code_unit() => ExecutionTestFromFile("return-first-code-unit");
    [Fact(DisplayName = "return-single-code-unit.js")]
    public Task return_single_code_unit() => ExecutionTestFromFile("return-single-code-unit");
    [Fact(DisplayName = "return-utf16-decode.js")]
    public Task return_utf16_decode() => ExecutionTestFromFile("return-utf16-decode");
    [Fact(DisplayName = "returns-undefined-on-position-equal-or-more-than-size.js")]
    public Task returns_undefined_on_position_equal_or_more_than_size() => ExecutionTestFromFile("returns-undefined-on-position-equal-or-more-than-size");
    [Fact(DisplayName = "returns-undefined-on-position-less-than-zero.js")]
    public Task returns_undefined_on_position_less_than_zero() => ExecutionTestFromFile("returns-undefined-on-position-less-than-zero");
    [Fact(DisplayName = "this-is-null-throws.js")]
    public Task this_is_null_throws() => ExecutionTestFromFile("this-is-null-throws");
    [Fact(DisplayName = "this-is-undefined-throws.js")]
    public Task this_is_undefined_throws() => ExecutionTestFromFile("this-is-undefined-throws");
}
