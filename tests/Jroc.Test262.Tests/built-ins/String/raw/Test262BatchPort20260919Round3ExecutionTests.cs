using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.raw;

public class Test262BatchPort20260919Round3ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919Round3ExecutionTests() : base("built_ins.String.raw") { }

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "raw")]
    public Task raw()
        => ExecutionTestFromFile("raw");

    [Fact(DisplayName = "return-empty-string-from-empty-array-length")]
    public Task return_empty_string_from_empty_array_length()
        => ExecutionTestFromFile("return-empty-string-from-empty-array-length");

    [Fact(DisplayName = "return-empty-string-if-length-is-negative-infinity")]
    public Task return_empty_string_if_length_is_negative_infinity()
        => ExecutionTestFromFile("return-empty-string-if-length-is-negative-infinity");

    [Fact(DisplayName = "return-empty-string-if-length-is-not-defined")]
    public Task return_empty_string_if_length_is_not_defined()
        => ExecutionTestFromFile("return-empty-string-if-length-is-not-defined");

    [Fact(DisplayName = "return-empty-string-if-length-is-undefined")]
    public Task return_empty_string_if_length_is_undefined()
        => ExecutionTestFromFile("return-empty-string-if-length-is-undefined");

    [Fact(DisplayName = "return-empty-string-if-length-is-zero-NaN")]
    public Task return_empty_string_if_length_is_zero_NaN()
        => ExecutionTestFromFile("return-empty-string-if-length-is-zero-NaN");

    [Fact(DisplayName = "return-empty-string-if-length-is-zero-boolean")]
    public Task return_empty_string_if_length_is_zero_boolean()
        => ExecutionTestFromFile("return-empty-string-if-length-is-zero-boolean");

    [Fact(DisplayName = "return-empty-string-if-length-is-zero-null")]
    public Task return_empty_string_if_length_is_zero_null()
        => ExecutionTestFromFile("return-empty-string-if-length-is-zero-null");

    [Fact(DisplayName = "return-empty-string-if-length-is-zero-or-less-number")]
    public Task return_empty_string_if_length_is_zero_or_less_number()
        => ExecutionTestFromFile("return-empty-string-if-length-is-zero-or-less-number");

    [Fact(DisplayName = "return-empty-string-if-length-is-zero-or-less-string")]
    public Task return_empty_string_if_length_is_zero_or_less_string()
        => ExecutionTestFromFile("return-empty-string-if-length-is-zero-or-less-string");

    [Fact(DisplayName = "return-the-string-value-from-template")]
    public Task return_the_string_value_from_template()
        => ExecutionTestFromFile("return-the-string-value-from-template");

    [Fact(DisplayName = "returns-abrupt-from-substitution")]
    public Task returns_abrupt_from_substitution()
        => ExecutionTestFromFile("returns-abrupt-from-substitution");

    [Fact(DisplayName = "special-characters")]
    public Task special_characters()
        => ExecutionTestFromFile("special-characters");

    [Fact(DisplayName = "substitutions-are-limited-to-template-raw-length")]
    public Task substitutions_are_limited_to_template_raw_length()
        => ExecutionTestFromFile("substitutions-are-limited-to-template-raw-length");

    [Fact(DisplayName = "template-not-object-throws")]
    public Task template_not_object_throws()
        => ExecutionTestFromFile("template-not-object-throws");

    [Fact(DisplayName = "template-raw-not-object-throws")]
    public Task template_raw_not_object_throws()
        => ExecutionTestFromFile("template-raw-not-object-throws");

    [Fact(DisplayName = "template-raw-throws")]
    public Task template_raw_throws()
        => ExecutionTestFromFile("template-raw-throws");

    [Fact(DisplayName = "template-substitutions-are-appended-on-same-index")]
    public Task template_substitutions_are_appended_on_same_index()
        => ExecutionTestFromFile("template-substitutions-are-appended-on-same-index");

    [Fact(DisplayName = "zero-literal-segments")]
    public Task zero_literal_segments()
        => ExecutionTestFromFile("zero-literal-segments");

}
