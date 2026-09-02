using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.ctors_bigint.object_arg;

public class TypedArrayConstructorsConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConstructorsConformanceBatchExecutionTests() : base("built_ins.TypedArrayConstructors.ctors_bigint.object_arg") { }

    [Fact(DisplayName = "as-array-returns.js")]
    public Task as_array_returns() => ExecutionTestFromFile("as-array-returns");

    [Fact(DisplayName = "as-generator-iterable-returns.js")]
    public Task as_generator_iterable_returns() => ExecutionTestFromFile("as-generator-iterable-returns");

    [Fact(DisplayName = "bigint-tobigint64.js")]
    public Task bigint_tobigint64() => ExecutionTestFromFile("bigint-tobigint64");

    [Fact(DisplayName = "bigint-tobiguint64.js")]
    public Task bigint_tobiguint64() => ExecutionTestFromFile("bigint-tobiguint64");

    [Fact(DisplayName = "boolean-tobigint.js")]
    public Task boolean_tobigint() => ExecutionTestFromFile("boolean-tobigint");

    [Fact(DisplayName = "custom-proto-access-throws.js")]
    public Task custom_proto_access_throws() => ExecutionTestFromFile("custom-proto-access-throws");

    [Fact(DisplayName = "iterating-throws.js")]
    public Task iterating_throws() => ExecutionTestFromFile("iterating-throws");

    [Fact(DisplayName = "iterator-not-callable-throws.js")]
    public Task iterator_not_callable_throws() => ExecutionTestFromFile("iterator-not-callable-throws");

    [Fact(DisplayName = "iterator-throws.js")]
    public Task iterator_throws() => ExecutionTestFromFile("iterator-throws");

    [Fact(DisplayName = "length-is-symbol-throws.js")]
    public Task length_is_symbol_throws() => ExecutionTestFromFile("length-is-symbol-throws");

    [Fact(DisplayName = "length-throws.js")]
    public Task length_throws() => ExecutionTestFromFile("length-throws");

    [Fact(DisplayName = "new-instance-extensibility.js")]
    public Task new_instance_extensibility() => ExecutionTestFromFile("new-instance-extensibility");

    [Fact(DisplayName = "null-tobigint.js")]
    public Task null_tobigint() => ExecutionTestFromFile("null-tobigint");

    [Fact(DisplayName = "number-tobigint.js")]
    public Task number_tobigint() => ExecutionTestFromFile("number-tobigint");

    [Fact(DisplayName = "string-nan-tobigint.js")]
    public Task string_nan_tobigint() => ExecutionTestFromFile("string-nan-tobigint");

    [Fact(DisplayName = "string-tobigint.js")]
    public Task string_tobigint() => ExecutionTestFromFile("string-tobigint");

    [Fact(DisplayName = "symbol-tobigint.js")]
    public Task symbol_tobigint() => ExecutionTestFromFile("symbol-tobigint");

    [Fact(DisplayName = "throws-from-property.js")]
    public Task throws_from_property() => ExecutionTestFromFile("throws-from-property");

    [Fact(DisplayName = "throws-setting-obj-to-primitive-typeerror.js")]
    public Task throws_setting_obj_to_primitive_typeerror() => ExecutionTestFromFile("throws-setting-obj-to-primitive-typeerror");

    [Fact(DisplayName = "throws-setting-obj-to-primitive.js")]
    public Task throws_setting_obj_to_primitive() => ExecutionTestFromFile("throws-setting-obj-to-primitive");

    [Fact(DisplayName = "throws-setting-obj-tostring.js")]
    public Task throws_setting_obj_tostring() => ExecutionTestFromFile("throws-setting-obj-tostring");

    [Fact(DisplayName = "throws-setting-obj-valueof-typeerror.js")]
    public Task throws_setting_obj_valueof_typeerror() => ExecutionTestFromFile("throws-setting-obj-valueof-typeerror");

    [Fact(DisplayName = "throws-setting-obj-valueof.js")]
    public Task throws_setting_obj_valueof() => ExecutionTestFromFile("throws-setting-obj-valueof");

    [Fact(DisplayName = "throws-setting-property.js")]
    public Task throws_setting_property() => ExecutionTestFromFile("throws-setting-property");

    [Fact(DisplayName = "throws-setting-symbol-property.js")]
    public Task throws_setting_symbol_property() => ExecutionTestFromFile("throws-setting-symbol-property");

    [Fact(DisplayName = "undefined-tobigint.js")]
    public Task undefined_tobigint() => ExecutionTestFromFile("undefined-tobigint");

    [Fact(DisplayName = "use-custom-proto-if-object.js")]
    public Task use_custom_proto_if_object() => ExecutionTestFromFile("use-custom-proto-if-object");

    [Fact(DisplayName = "use-default-proto-if-custom-proto-is-not-object.js")]
    public Task use_default_proto_if_custom_proto_is_not_object() => ExecutionTestFromFile("use-default-proto-if-custom-proto-is-not-object");

}
