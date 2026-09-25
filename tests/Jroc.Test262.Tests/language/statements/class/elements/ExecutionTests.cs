using Jroc.Tests;

namespace Jroc.Test262.Tests.language.statements.class_.elements;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.elements") { }

    [Fact(DisplayName = "static-private-setter-access-on-inner-function.js")]
    public Task ported_static_private_setter_access_on_inner_function() => ExecutionTest("static-private-setter-access-on-inner-function");

    [Fact(DisplayName = "static-private-setter.js")]
    public Task ported_static_private_setter() => ExecutionTest("static-private-setter");

    [Fact(DisplayName = "private-setter-access-on-inner-function.js")]
    public Task ported_private_setter_access_on_inner_function() => ExecutionTest("private-setter-access-on-inner-function");

    [Fact(DisplayName = "private-setter-brand-check.js")]
    public Task ported_private_setter_brand_check() => ExecutionTest("private-setter-brand-check");

    [Fact(DisplayName = "private-setter-shadowed-by-setter-on-nested-class.js")]
    public Task ported_private_setter_shadowed_by_setter_on_nested_class() => ExecutionTest("private-setter-shadowed-by-setter-on-nested-class");

    [Fact(DisplayName = "private-static-field-shadowed-by-setter-on-nested-class.js")]
    public Task ported_private_static_field_shadowed_by_setter_on_nested_class() => ExecutionTest("private-static-field-shadowed-by-setter-on-nested-class");

    [Fact(DisplayName = "after-same-line-gen-private-method-getter-usage.js")]
    public Task ported_after_same_line_gen_private_method_getter_usage() => ExecutionTest("after-same-line-gen-private-method-getter-usage");

    [Fact(DisplayName = "after-same-line-method-private-method-getter-usage.js")]
    public Task ported_after_same_line_method_private_method_getter_usage() => ExecutionTest("after-same-line-method-private-method-getter-usage");

    [Fact(DisplayName = "after-same-line-static-async-gen-private-method-getter-usage.js")]
    public Task ported_after_same_line_static_async_gen_private_method_getter_usage() => ExecutionTest("after-same-line-static-async-gen-private-method-getter-usage");

    [Fact(DisplayName = "after-same-line-static-async-method-private-method-getter-usage.js")]
    public Task ported_after_same_line_static_async_method_private_method_getter_usage() => ExecutionTest("after-same-line-static-async-method-private-method-getter-usage");

    [Fact(DisplayName = "after-same-line-static-gen-private-method-getter-usage.js")]
    public Task ported_after_same_line_static_gen_private_method_getter_usage() => ExecutionTest("after-same-line-static-gen-private-method-getter-usage");

    [Fact(DisplayName = "after-same-line-static-method-private-method-getter-usage.js")]
    public Task ported_after_same_line_static_method_private_method_getter_usage() => ExecutionTest("after-same-line-static-method-private-method-getter-usage");

    [Fact(DisplayName = "multiple-definitions-private-method-getter-usage.js")]
    public Task ported_multiple_definitions_private_method_getter_usage() => ExecutionTest("multiple-definitions-private-method-getter-usage");

    [Fact(DisplayName = "private-setter-shadowed-by-getter-on-nested-class.js")]
    public Task ported_private_setter_shadowed_by_getter_on_nested_class() => ExecutionTest("private-setter-shadowed-by-getter-on-nested-class");

    [Fact(DisplayName = "private-setter-shadowed-by-method-on-nested-class.js")]
    public Task ported_private_setter_shadowed_by_method_on_nested_class() => ExecutionTest("private-setter-shadowed-by-method-on-nested-class");

    [Fact(DisplayName = "private-setter-shadowed-by-field-on-nested-class.js")]
    public Task ported_private_setter_shadowed_by_field_on_nested_class() => ExecutionTest("private-setter-shadowed-by-field-on-nested-class");

    [Fact(DisplayName = "private-method-shadowed-by-field-on-nested-class.js")]
    public Task ported_private_method_shadowed_by_field_on_nested_class() => ExecutionTest("private-method-shadowed-by-field-on-nested-class");

    [Fact(DisplayName = "private-static-field-shadowed-by-field-on-nested-class.js")]
    public Task ported_private_static_field_shadowed_by_field_on_nested_class() => ExecutionTest("private-static-field-shadowed-by-field-on-nested-class");

    [Fact(DisplayName = "private-getter-shadowed-by-field-on-nested-class.js")]
    public Task ported_private_getter_shadowed_by_field_on_nested_class() => ExecutionTest("private-getter-shadowed-by-field-on-nested-class");

    [Fact(DisplayName = "private-method-shadowed-on-nested-class.js")]
    public Task ported_private_method_shadowed_on_nested_class() => ExecutionTest("private-method-shadowed-on-nested-class");

    [Fact(DisplayName = "after-same-line-gen-rs-static-privatename-identifier.js")]
    public Task ported_after_same_line_gen_rs_static_privatename_identifier() => ExecutionTest("after-same-line-gen-rs-static-privatename-identifier");

    [Fact(DisplayName = "after-same-line-gen-rs-static-privatename-identifier-alt.js")]
    public Task ported_after_same_line_gen_rs_static_privatename_identifier_alt() => ExecutionTest("after-same-line-gen-rs-static-privatename-identifier-alt");

    [Fact(DisplayName = "after-same-line-gen-rs-static-privatename-identifier-initializer.js")]
    public Task ported_after_same_line_gen_rs_static_privatename_identifier_initializer() => ExecutionTest("after-same-line-gen-rs-static-privatename-identifier-initializer");

    [Fact(DisplayName = "after-same-line-gen-rs-static-privatename-identifier-initializer-alt.js")]
    public Task ported_after_same_line_gen_rs_static_privatename_identifier_initializer_alt() => ExecutionTest("after-same-line-gen-rs-static-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "after-same-line-method-rs-static-privatename-identifier.js")]
    public Task ported_after_same_line_method_rs_static_privatename_identifier() => ExecutionTest("after-same-line-method-rs-static-privatename-identifier");

    [Fact(DisplayName = "after-same-line-method-rs-static-privatename-identifier-alt.js")]
    public Task ported_after_same_line_method_rs_static_privatename_identifier_alt() => ExecutionTest("after-same-line-method-rs-static-privatename-identifier-alt");

    [Fact(DisplayName = "after-same-line-method-rs-static-privatename-identifier-initializer.js")]
    public Task ported_after_same_line_method_rs_static_privatename_identifier_initializer() => ExecutionTest("after-same-line-method-rs-static-privatename-identifier-initializer");

    [Fact(DisplayName = "after-same-line-method-rs-static-privatename-identifier-initializer-alt.js")]
    public Task ported_after_same_line_method_rs_static_privatename_identifier_initializer_alt() => ExecutionTest("after-same-line-method-rs-static-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "after-same-line-static-async-gen-rs-static-privatename-identifier.js")]
    public Task ported_after_same_line_static_async_gen_rs_static_privatename_identifier() => ExecutionTest("after-same-line-static-async-gen-rs-static-privatename-identifier");

    [Fact(DisplayName = "after-same-line-static-async-gen-rs-static-privatename-identifier-alt.js")]
    public Task ported_after_same_line_static_async_gen_rs_static_privatename_identifier_alt() => ExecutionTest("after-same-line-static-async-gen-rs-static-privatename-identifier-alt");

    [Fact(DisplayName = "after-same-line-static-async-gen-rs-static-privatename-identifier-initializer.js")]
    public Task ported_after_same_line_static_async_gen_rs_static_privatename_identifier_initializer() => ExecutionTest("after-same-line-static-async-gen-rs-static-privatename-identifier-initializer");

    [Fact(DisplayName = "after-same-line-static-async-gen-rs-static-privatename-identifier-initializer-alt.js")]
    public Task ported_after_same_line_static_async_gen_rs_static_privatename_identifier_initializer_alt() => ExecutionTest("after-same-line-static-async-gen-rs-static-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "after-same-line-static-async-method-rs-static-privatename-identifier.js")]
    public Task ported_after_same_line_static_async_method_rs_static_privatename_identifier() => ExecutionTest("after-same-line-static-async-method-rs-static-privatename-identifier");

    [Fact(DisplayName = "after-same-line-static-async-method-rs-static-privatename-identifier-alt.js")]
    public Task ported_after_same_line_static_async_method_rs_static_privatename_identifier_alt() => ExecutionTest("after-same-line-static-async-method-rs-static-privatename-identifier-alt");

    [Fact(DisplayName = "after-same-line-static-async-method-rs-static-privatename-identifier-initializer.js")]
    public Task ported_after_same_line_static_async_method_rs_static_privatename_identifier_initializer() => ExecutionTest("after-same-line-static-async-method-rs-static-privatename-identifier-initializer");

    [Fact(DisplayName = "after-same-line-static-async-method-rs-static-privatename-identifier-initializer-alt.js")]
    public Task ported_after_same_line_static_async_method_rs_static_privatename_identifier_initializer_alt() => ExecutionTest("after-same-line-static-async-method-rs-static-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "after-same-line-static-gen-rs-static-privatename-identifier.js")]
    public Task ported_after_same_line_static_gen_rs_static_privatename_identifier() => ExecutionTest("after-same-line-static-gen-rs-static-privatename-identifier");

    [Fact(DisplayName = "after-same-line-static-gen-rs-static-privatename-identifier-alt.js")]
    public Task ported_after_same_line_static_gen_rs_static_privatename_identifier_alt() => ExecutionTest("after-same-line-static-gen-rs-static-privatename-identifier-alt");

    [Fact(DisplayName = "after-same-line-static-gen-rs-static-privatename-identifier-initializer.js")]
    public Task ported_after_same_line_static_gen_rs_static_privatename_identifier_initializer() => ExecutionTest("after-same-line-static-gen-rs-static-privatename-identifier-initializer");

    [Fact(DisplayName = "after-same-line-static-gen-rs-static-privatename-identifier-initializer-alt.js")]
    public Task ported_after_same_line_static_gen_rs_static_privatename_identifier_initializer_alt() => ExecutionTest("after-same-line-static-gen-rs-static-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "after-same-line-static-method-rs-static-privatename-identifier.js")]
    public Task ported_after_same_line_static_method_rs_static_privatename_identifier() => ExecutionTest("after-same-line-static-method-rs-static-privatename-identifier");

    [Fact(DisplayName = "after-same-line-static-method-rs-static-privatename-identifier-alt.js")]
    public Task ported_after_same_line_static_method_rs_static_privatename_identifier_alt() => ExecutionTest("after-same-line-static-method-rs-static-privatename-identifier-alt");

    [Fact(DisplayName = "after-same-line-static-method-rs-static-privatename-identifier-initializer.js")]
    public Task ported_after_same_line_static_method_rs_static_privatename_identifier_initializer() => ExecutionTest("after-same-line-static-method-rs-static-privatename-identifier-initializer");

    [Fact(DisplayName = "after-same-line-static-method-rs-static-privatename-identifier-initializer-alt.js")]
    public Task ported_after_same_line_static_method_rs_static_privatename_identifier_initializer_alt() => ExecutionTest("after-same-line-static-method-rs-static-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "multiple-definitions-rs-static-privatename-identifier.js")]
    public Task ported_multiple_definitions_rs_static_privatename_identifier() => ExecutionTest("multiple-definitions-rs-static-privatename-identifier");

    [Fact(DisplayName = "multiple-definitions-rs-static-privatename-identifier-alt.js")]
    public Task ported_multiple_definitions_rs_static_privatename_identifier_alt() => ExecutionTest("multiple-definitions-rs-static-privatename-identifier-alt");

    [Fact(DisplayName = "multiple-definitions-rs-static-privatename-identifier-initializer.js")]
    public Task ported_multiple_definitions_rs_static_privatename_identifier_initializer() => ExecutionTest("multiple-definitions-rs-static-privatename-identifier-initializer");

    [Fact(DisplayName = "multiple-definitions-rs-static-privatename-identifier-initializer-alt.js")]
    public Task ported_multiple_definitions_rs_static_privatename_identifier_initializer_alt() => ExecutionTest("multiple-definitions-rs-static-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-static-privatename-identifier.js")]
    public Task ported_multiple_stacked_definitions_rs_static_privatename_identifier() => ExecutionTest("multiple-stacked-definitions-rs-static-privatename-identifier");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-static-privatename-identifier-alt.js")]
    public Task ported_multiple_stacked_definitions_rs_static_privatename_identifier_alt() => ExecutionTest("multiple-stacked-definitions-rs-static-privatename-identifier-alt");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-static-privatename-identifier-initializer.js")]
    public Task ported_multiple_stacked_definitions_rs_static_privatename_identifier_initializer() => ExecutionTest("multiple-stacked-definitions-rs-static-privatename-identifier-initializer");

    [Fact(DisplayName = "multiple-stacked-definitions-rs-static-privatename-identifier-initializer-alt.js")]
    public Task ported_multiple_stacked_definitions_rs_static_privatename_identifier_initializer_alt() => ExecutionTest("multiple-stacked-definitions-rs-static-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "new-no-sc-line-method-rs-static-privatename-identifier.js")]
    public Task ported_new_no_sc_line_method_rs_static_privatename_identifier() => ExecutionTest("new-no-sc-line-method-rs-static-privatename-identifier");

    [Fact(DisplayName = "new-no-sc-line-method-rs-static-privatename-identifier-alt.js")]
    public Task ported_new_no_sc_line_method_rs_static_privatename_identifier_alt() => ExecutionTest("new-no-sc-line-method-rs-static-privatename-identifier-alt");

    [Fact(DisplayName = "new-no-sc-line-method-rs-static-privatename-identifier-initializer.js")]
    public Task ported_new_no_sc_line_method_rs_static_privatename_identifier_initializer() => ExecutionTest("new-no-sc-line-method-rs-static-privatename-identifier-initializer");

    [Fact(DisplayName = "new-no-sc-line-method-rs-static-privatename-identifier-initializer-alt.js")]
    public Task ported_new_no_sc_line_method_rs_static_privatename_identifier_initializer_alt() => ExecutionTest("new-no-sc-line-method-rs-static-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "new-sc-line-gen-rs-static-privatename-identifier.js")]
    public Task ported_new_sc_line_gen_rs_static_privatename_identifier() => ExecutionTest("new-sc-line-gen-rs-static-privatename-identifier");

    [Fact(DisplayName = "new-sc-line-gen-rs-static-privatename-identifier-alt.js")]
    public Task ported_new_sc_line_gen_rs_static_privatename_identifier_alt() => ExecutionTest("new-sc-line-gen-rs-static-privatename-identifier-alt");

    [Fact(DisplayName = "new-sc-line-gen-rs-static-privatename-identifier-initializer.js")]
    public Task ported_new_sc_line_gen_rs_static_privatename_identifier_initializer() => ExecutionTest("new-sc-line-gen-rs-static-privatename-identifier-initializer");

    [Fact(DisplayName = "new-sc-line-gen-rs-static-privatename-identifier-initializer-alt.js")]
    public Task ported_new_sc_line_gen_rs_static_privatename_identifier_initializer_alt() => ExecutionTest("new-sc-line-gen-rs-static-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "new-sc-line-method-rs-static-privatename-identifier.js")]
    public Task ported_new_sc_line_method_rs_static_privatename_identifier() => ExecutionTest("new-sc-line-method-rs-static-privatename-identifier");

    [Fact(DisplayName = "new-sc-line-method-rs-static-privatename-identifier-alt.js")]
    public Task ported_new_sc_line_method_rs_static_privatename_identifier_alt() => ExecutionTest("new-sc-line-method-rs-static-privatename-identifier-alt");

    [Fact(DisplayName = "new-sc-line-method-rs-static-privatename-identifier-initializer.js")]
    public Task ported_new_sc_line_method_rs_static_privatename_identifier_initializer() => ExecutionTest("new-sc-line-method-rs-static-privatename-identifier-initializer");

    [Fact(DisplayName = "new-sc-line-method-rs-static-privatename-identifier-initializer-alt.js")]
    public Task ported_new_sc_line_method_rs_static_privatename_identifier_initializer_alt() => ExecutionTest("new-sc-line-method-rs-static-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "regular-definitions-rs-static-privatename-identifier.js")]
    public Task ported_regular_definitions_rs_static_privatename_identifier() => ExecutionTest("regular-definitions-rs-static-privatename-identifier");

    [Fact(DisplayName = "regular-definitions-rs-static-privatename-identifier-alt.js")]
    public Task ported_regular_definitions_rs_static_privatename_identifier_alt() => ExecutionTest("regular-definitions-rs-static-privatename-identifier-alt");

    [Fact(DisplayName = "regular-definitions-rs-static-privatename-identifier-initializer.js")]
    public Task ported_regular_definitions_rs_static_privatename_identifier_initializer() => ExecutionTest("regular-definitions-rs-static-privatename-identifier-initializer");

    [Fact(DisplayName = "regular-definitions-rs-static-privatename-identifier-initializer-alt.js")]
    public Task ported_regular_definitions_rs_static_privatename_identifier_initializer_alt() => ExecutionTest("regular-definitions-rs-static-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "same-line-async-gen-rs-static-privatename-identifier-alt.js")]
    public Task ported_same_line_async_gen_rs_static_privatename_identifier_alt() => ExecutionTest("same-line-async-gen-rs-static-privatename-identifier-alt");

    [Fact(DisplayName = "same-line-async-gen-rs-static-privatename-identifier-initializer-alt.js")]
    public Task ported_same_line_async_gen_rs_static_privatename_identifier_initializer_alt() => ExecutionTest("same-line-async-gen-rs-static-privatename-identifier-initializer-alt");

    [Fact(DisplayName = "abrupt-completition-on-field-initializer")]
    public Task abrupt_completition_on_field_initializer()
        => ExecutionTest("abrupt-completition-on-field-initializer");

    [Fact(DisplayName = "after-same-line-static-gen-private-method-usage")]
    public Task after_same_line_static_gen_private_method_usage()
        => ExecutionTest("after-same-line-static-gen-private-method-usage");

    [Fact(DisplayName = "after-same-line-static-method-private-method-usage")]
    public Task after_same_line_static_method_private_method_usage()
        => ExecutionTest("after-same-line-static-method-private-method-usage");

    [Fact(DisplayName = "after-same-line-static-method-private-names")]
    public Task after_same_line_static_method_private_names()
        => ExecutionTest("after-same-line-static-method-private-names");

    [Fact(DisplayName = "class-field-is-observable-by-proxy")]
    public Task class_field_is_observable_by_proxy()
        => ExecutionTest("class-field-is-observable-by-proxy");

    [Fact(DisplayName = "multiple-definitions-private-method-usage")]
    public Task multiple_definitions_private_method_usage()
        => ExecutionTest("multiple-definitions-private-method-usage");

    [Fact(DisplayName = "multiple-definitions-private-names")]
    public Task multiple_definitions_private_names()
        => ExecutionTest("multiple-definitions-private-names");

    [Fact(DisplayName = "new-sc-line-method-rs-static-method-privatename-identifier-alt")]
    public Task new_sc_line_method_rs_static_method_privatename_identifier_alt()
        => ExecutionTest("new-sc-line-method-rs-static-method-privatename-identifier-alt");

    [Fact(DisplayName = "after-same-line-static-gen-computed-names")]
    public Task after_same_line_static_gen_computed_names()
        => ExecutionTest("after-same-line-static-gen-computed-names");

    [Fact(DisplayName = "after-same-line-gen-string-literal-names.js")]
    public Task ported_after_same_line_gen_string_literal_names() => ExecutionTest("after-same-line-gen-string-literal-names");

    [Fact(DisplayName = "after-same-line-method-string-literal-names.js")]
    public Task ported_after_same_line_method_string_literal_names() => ExecutionTest("after-same-line-method-string-literal-names");

    [Fact(DisplayName = "after-same-line-static-gen-string-literal-names.js")]
    public Task ported_after_same_line_static_gen_string_literal_names() => ExecutionTest("after-same-line-static-gen-string-literal-names");

    [Fact(DisplayName = "after-same-line-static-method-string-literal-names.js")]
    public Task ported_after_same_line_static_method_string_literal_names() => ExecutionTest("after-same-line-static-method-string-literal-names");

    [Fact(DisplayName = "multiple-definitions-string-literal-names.js")]
    public Task ported_multiple_definitions_string_literal_names() => ExecutionTest("multiple-definitions-string-literal-names");

    [Fact(DisplayName = "multiple-stacked-definitions-string-literal-names.js")]
    public Task ported_multiple_stacked_definitions_string_literal_names() => ExecutionTest("multiple-stacked-definitions-string-literal-names");

    [Fact(DisplayName = "new-no-sc-line-method-string-literal-names.js")]
    public Task ported_new_no_sc_line_method_string_literal_names() => ExecutionTest("new-no-sc-line-method-string-literal-names");

    [Fact(DisplayName = "new-sc-line-gen-string-literal-names.js")]
    public Task ported_new_sc_line_gen_string_literal_names() => ExecutionTest("new-sc-line-gen-string-literal-names");

    [Fact(DisplayName = "new-sc-line-method-string-literal-names.js")]
    public Task ported_new_sc_line_method_string_literal_names() => ExecutionTest("new-sc-line-method-string-literal-names");

    [Fact(DisplayName = "regular-definitions-string-literal-names.js")]
    public Task ported_regular_definitions_string_literal_names() => ExecutionTest("regular-definitions-string-literal-names");

    [Fact(DisplayName = "same-line-gen-string-literal-names.js")]
    public Task ported_same_line_gen_string_literal_names() => ExecutionTest("same-line-gen-string-literal-names");

    [Fact(DisplayName = "same-line-method-string-literal-names.js")]
    public Task ported_same_line_method_string_literal_names() => ExecutionTest("same-line-method-string-literal-names");

    [Fact(DisplayName = "wrapped-in-sc-string-literal-names.js")]
    public Task ported_wrapped_in_sc_string_literal_names() => ExecutionTest("wrapped-in-sc-string-literal-names");
}
