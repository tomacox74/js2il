namespace Jroc.Test262.Tests.built_ins.JSON.parse;

public partial class ExecutionTests
{
    [Fact(DisplayName = "15.12.2-2-10")]
    public Task _15_12_2_2_10() => ExecutionTestFromFile("15.12.2-2-10");

    [Fact(DisplayName = "15.12.2-2-2")]
    public Task _15_12_2_2_2() => ExecutionTestFromFile("15.12.2-2-2");

    [Fact(DisplayName = "15.12.2-2-3")]
    public Task _15_12_2_2_3() => ExecutionTestFromFile("15.12.2-2-3");

    [Fact(DisplayName = "15.12.2-2-4")]
    public Task _15_12_2_2_4() => ExecutionTestFromFile("15.12.2-2-4");

    [Fact(DisplayName = "15.12.2-2-5")]
    public Task _15_12_2_2_5() => ExecutionTestFromFile("15.12.2-2-5");

    [Fact(DisplayName = "15.12.2-2-6")]
    public Task _15_12_2_2_6() => ExecutionTestFromFile("15.12.2-2-6");

    [Fact(DisplayName = "15.12.2-2-7")]
    public Task _15_12_2_2_7() => ExecutionTestFromFile("15.12.2-2-7");

    [Fact(DisplayName = "15.12.2-2-8")]
    public Task _15_12_2_2_8() => ExecutionTestFromFile("15.12.2-2-8");

    [Fact(DisplayName = "15.12.2-2-9")]
    public Task _15_12_2_2_9() => ExecutionTestFromFile("15.12.2-2-9");

    [Fact(DisplayName = "duplicate-proto")]
    public Task duplicate_proto() => ExecutionTestFromFile("duplicate-proto");

    [Fact(DisplayName = "invalid-whitespace")]
    public Task invalid_whitespace() => ExecutionTestFromFile("invalid-whitespace");

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "revived-proxy")]
    public Task revived_proxy() => ExecutionTestFromFile("revived-proxy");

    [Fact(DisplayName = "revived-proxy-revoked")]
    public Task revived_proxy_revoked() => ExecutionTestFromFile("revived-proxy-revoked");

    [Fact(DisplayName = "reviver-array-define-prop-err")]
    public Task reviver_array_define_prop_err() => ExecutionTestFromFile("reviver-array-define-prop-err");

    [Fact(DisplayName = "reviver-array-delete-err")]
    public Task reviver_array_delete_err() => ExecutionTestFromFile("reviver-array-delete-err");

    [Fact(DisplayName = "reviver-array-get-prop-from-prototype")]
    public Task reviver_array_get_prop_from_prototype() => ExecutionTestFromFile("reviver-array-get-prop-from-prototype");

    [Fact(DisplayName = "reviver-array-length-coerce-err")]
    public Task reviver_array_length_coerce_err() => ExecutionTestFromFile("reviver-array-length-coerce-err");

    [Fact(DisplayName = "reviver-array-length-get-err")]
    public Task reviver_array_length_get_err() => ExecutionTestFromFile("reviver-array-length-get-err");

    [Fact(DisplayName = "reviver-call-err")]
    public Task reviver_call_err() => ExecutionTestFromFile("reviver-call-err");

    [Fact(DisplayName = "reviver-context-source-primitive-literal")]
    public Task reviver_context_source_primitive_literal() => ExecutionTestFromFile("reviver-context-source-primitive-literal");

    [Fact(DisplayName = "reviver-get-name-err")]
    public Task reviver_get_name_err() => ExecutionTestFromFile("reviver-get-name-err");

    [Fact(DisplayName = "reviver-object-define-prop-err")]
    public Task reviver_object_define_prop_err() => ExecutionTestFromFile("reviver-object-define-prop-err");

    [Fact(DisplayName = "reviver-object-delete-err")]
    public Task reviver_object_delete_err() => ExecutionTestFromFile("reviver-object-delete-err");

    [Fact(DisplayName = "reviver-object-get-prop-from-prototype")]
    public Task reviver_object_get_prop_from_prototype() => ExecutionTestFromFile("reviver-object-get-prop-from-prototype");

    [Fact(DisplayName = "reviver-object-non-configurable-prop-create")]
    public Task reviver_object_non_configurable_prop_create() => ExecutionTestFromFile("reviver-object-non-configurable-prop-create");

    [Fact(DisplayName = "reviver-object-non-configurable-prop-delete")]
    public Task reviver_object_non_configurable_prop_delete() => ExecutionTestFromFile("reviver-object-non-configurable-prop-delete");

    [Fact(DisplayName = "reviver-object-own-keys-err")]
    public Task reviver_object_own_keys_err() => ExecutionTestFromFile("reviver-object-own-keys-err");

    [Fact(DisplayName = "S15.12.2_A1")]
    public Task S15_12_2_A1() => ExecutionTestFromFile("S15.12.2_A1");

    [Fact(DisplayName = "text-negative-zero")]
    public Task text_negative_zero() => ExecutionTestFromFile("text-negative-zero");

    [Fact(DisplayName = "text-non-string-primitive")]
    public Task text_non_string_primitive() => ExecutionTestFromFile("text-non-string-primitive");

    [Fact(DisplayName = "text-object-abrupt")]
    public Task text_object_abrupt() => ExecutionTestFromFile("text-object-abrupt");

    [Fact(DisplayName = "text-object")]
    public Task text_object() => ExecutionTestFromFile("text-object");
}
