using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.ctors.object_arg;

public class TypedArrayConstructorsConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConstructorsConformanceBatchExecutionTests() : base("built_ins.TypedArrayConstructors.ctors.object_arg") { }

    [Fact(DisplayName = "as-array-returns.js")]
    public Task as_array_returns() => ExecutionTestFromFile("as-array-returns");

    [Fact(DisplayName = "as-generator-iterable-returns.js")]
    public Task as_generator_iterable_returns() => ExecutionTestFromFile("as-generator-iterable-returns");

    [Fact(DisplayName = "conversion-operation-consistent-nan.js")]
    public Task conversion_operation_consistent_nan() => ExecutionTestFromFile("conversion-operation-consistent-nan");

    [Fact(DisplayName = "custom-proto-access-throws.js")]
    public Task custom_proto_access_throws() => ExecutionTestFromFile("custom-proto-access-throws");

    [Fact(DisplayName = "iterated-array-changed-by-tonumber.js")]
    public Task iterated_array_changed_by_tonumber() => ExecutionTestFromFile("iterated-array-changed-by-tonumber");

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

    [Fact(DisplayName = "returns.js")]
    public Task returns() => ExecutionTestFromFile("returns");

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

    [Fact(DisplayName = "use-custom-proto-if-object.js")]
    public Task use_custom_proto_if_object() => ExecutionTestFromFile("use-custom-proto-if-object");

    [Fact(DisplayName = "use-default-proto-if-custom-proto-is-not-object.js")]
    public Task use_default_proto_if_custom_proto_is_not_object() => ExecutionTestFromFile("use-default-proto-if-custom-proto-is-not-object");

}
