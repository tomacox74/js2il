using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView;

public class PortNext200Batch2ExecutionTests : DiskExecutionTestsBase
{
    public PortNext200Batch2ExecutionTests() : base("built_ins.DataView") { }

    [Fact(DisplayName = "buffer-does-not-have-arraybuffer-data-throws")]
    public Task buffer_does_not_have_arraybuffer_data_throws()
        => ExecutionTestFromFile("buffer-does-not-have-arraybuffer-data-throws");

    [Fact(DisplayName = "buffer-not-object-throws")]
    public Task buffer_not_object_throws()
        => ExecutionTestFromFile("buffer-not-object-throws");

    [Fact(DisplayName = "buffer-reference")]
    public Task buffer_reference()
        => ExecutionTestFromFile("buffer-reference");

    [Fact(DisplayName = "byteOffset-validated-against-initial-buffer-length")]
    public Task byteOffset_validated_against_initial_buffer_length()
        => ExecutionTestFromFile("byteOffset-validated-against-initial-buffer-length");

    [Fact(DisplayName = "byteoffset-is-negative-throws")]
    public Task byteoffset_is_negative_throws()
        => ExecutionTestFromFile("byteoffset-is-negative-throws");

    [Fact(DisplayName = "prototype/setFloat64/no-value-arg")]
    public Task prototype_setFloat64_no_value_arg()
        => ExecutionTestFromFile("prototype/setFloat64/no-value-arg");

    [Fact(DisplayName = "custom-proto-if-not-object-fallbacks-to-default-prototype")]
    public Task custom_proto_if_not_object_fallbacks_to_default_prototype()
        => ExecutionTestFromFile("custom-proto-if-not-object-fallbacks-to-default-prototype");

    [Fact(DisplayName = "prototype/setFloat64/range-check-after-value-conversion")]
    public Task prototype_setFloat64_range_check_after_value_conversion()
        => ExecutionTestFromFile("prototype/setFloat64/range-check-after-value-conversion");

    [Fact(DisplayName = "defined-bytelength-and-byteoffset")]
    public Task defined_bytelength_and_byteoffset()
        => ExecutionTestFromFile("defined-bytelength-and-byteoffset");

    [Fact(DisplayName = "defined-byteoffset-undefined-bytelength")]
    public Task defined_byteoffset_undefined_bytelength()
        => ExecutionTestFromFile("defined-byteoffset-undefined-bytelength");

    [Fact(DisplayName = "defined-byteoffset")]
    public Task defined_byteoffset()
        => ExecutionTestFromFile("defined-byteoffset");

    [Fact(DisplayName = "excessive-bytelength-throws")]
    public Task excessive_bytelength_throws()
        => ExecutionTestFromFile("excessive-bytelength-throws");

    [Fact(DisplayName = "excessive-byteoffset-throws")]
    public Task excessive_byteoffset_throws()
        => ExecutionTestFromFile("excessive-byteoffset-throws");

    [Fact(DisplayName = "extensibility")]
    public Task extensibility()
        => ExecutionTestFromFile("extensibility");

    [Fact(DisplayName = "negative-bytelength-throws")]
    public Task negative_bytelength_throws()
        => ExecutionTestFromFile("negative-bytelength-throws");

    [Fact(DisplayName = "negative-byteoffset-throws")]
    public Task negative_byteoffset_throws()
        => ExecutionTestFromFile("negative-byteoffset-throws");

    [Fact(DisplayName = "prototype/buffer/invoked-as-accessor")]
    public Task prototype_buffer_invoked_as_accessor()
        => ExecutionTestFromFile("prototype/buffer/invoked-as-accessor");

    [Fact(DisplayName = "prototype/buffer/invoked-as-func")]
    public Task prototype_buffer_invoked_as_func()
        => ExecutionTestFromFile("prototype/buffer/invoked-as-func");

    [Fact(DisplayName = "prototype/buffer/return-buffer")]
    public Task prototype_buffer_return_buffer()
        => ExecutionTestFromFile("prototype/buffer/return-buffer");

    [Fact(DisplayName = "prototype/buffer/this-has-no-dataview-internal")]
    public Task prototype_buffer_this_has_no_dataview_internal()
        => ExecutionTestFromFile("prototype/buffer/this-has-no-dataview-internal");

    [Fact(DisplayName = "prototype/buffer/this-is-not-object")]
    public Task prototype_buffer_this_is_not_object()
        => ExecutionTestFromFile("prototype/buffer/this-is-not-object");

    [Fact(DisplayName = "prototype/byteLength/invoked-as-accessor")]
    public Task prototype_byteLength_invoked_as_accessor()
        => ExecutionTestFromFile("prototype/byteLength/invoked-as-accessor");

    [Fact(DisplayName = "prototype/byteLength/invoked-as-func")]
    public Task prototype_byteLength_invoked_as_func()
        => ExecutionTestFromFile("prototype/byteLength/invoked-as-func");

    [Fact(DisplayName = "prototype/byteLength/return-bytelength")]
    public Task prototype_byteLength_return_bytelength()
        => ExecutionTestFromFile("prototype/byteLength/return-bytelength");

    [Fact(DisplayName = "prototype/byteLength/this-has-no-dataview-internal")]
    public Task prototype_byteLength_this_has_no_dataview_internal()
        => ExecutionTestFromFile("prototype/byteLength/this-has-no-dataview-internal");

    [Fact(DisplayName = "prototype/byteLength/this-is-not-object")]
    public Task prototype_byteLength_this_is_not_object()
        => ExecutionTestFromFile("prototype/byteLength/this-is-not-object");

    [Fact(DisplayName = "prototype/byteOffset/invoked-as-accessor")]
    public Task prototype_byteOffset_invoked_as_accessor()
        => ExecutionTestFromFile("prototype/byteOffset/invoked-as-accessor");

    [Fact(DisplayName = "prototype/byteOffset/invoked-as-func")]
    public Task prototype_byteOffset_invoked_as_func()
        => ExecutionTestFromFile("prototype/byteOffset/invoked-as-func");

    [Fact(DisplayName = "prototype/byteOffset/return-byteoffset")]
    public Task prototype_byteOffset_return_byteoffset()
        => ExecutionTestFromFile("prototype/byteOffset/return-byteoffset");

    [Fact(DisplayName = "prototype/byteOffset/this-has-no-dataview-internal")]
    public Task prototype_byteOffset_this_has_no_dataview_internal()
        => ExecutionTestFromFile("prototype/byteOffset/this-has-no-dataview-internal");

    [Fact(DisplayName = "prototype/byteOffset/this-is-not-object")]
    public Task prototype_byteOffset_this_is_not_object()
        => ExecutionTestFromFile("prototype/byteOffset/this-is-not-object");

    [Fact(DisplayName = "prototype/getFloat32/index-is-out-of-range")]
    public Task prototype_getFloat32_index_is_out_of_range()
        => ExecutionTestFromFile("prototype/getFloat32/index-is-out-of-range");

    [Fact(DisplayName = "prototype/getFloat32/minus-zero")]
    public Task prototype_getFloat32_minus_zero()
        => ExecutionTestFromFile("prototype/getFloat32/minus-zero");

    [Fact(DisplayName = "prototype/getFloat32/negative-byteoffset-throws")]
    public Task prototype_getFloat32_negative_byteoffset_throws()
        => ExecutionTestFromFile("prototype/getFloat32/negative-byteoffset-throws");

    [Fact(DisplayName = "prototype/getFloat32/return-abrupt-from-tonumber-byteoffset-symbol")]
    public Task prototype_getFloat32_return_abrupt_from_tonumber_byteoffset_symbol()
        => ExecutionTestFromFile("prototype/getFloat32/return-abrupt-from-tonumber-byteoffset-symbol");

    [Fact(DisplayName = "prototype/getFloat32/return-abrupt-from-tonumber-byteoffset")]
    public Task prototype_getFloat32_return_abrupt_from_tonumber_byteoffset()
        => ExecutionTestFromFile("prototype/getFloat32/return-abrupt-from-tonumber-byteoffset");

    [Fact(DisplayName = "prototype/getFloat32/return-infinity")]
    public Task prototype_getFloat32_return_infinity()
        => ExecutionTestFromFile("prototype/getFloat32/return-infinity");

    [Fact(DisplayName = "prototype/getFloat32/return-nan")]
    public Task prototype_getFloat32_return_nan()
        => ExecutionTestFromFile("prototype/getFloat32/return-nan");

    [Fact(DisplayName = "prototype/getFloat32/return-value-clean-arraybuffer")]
    public Task prototype_getFloat32_return_value_clean_arraybuffer()
        => ExecutionTestFromFile("prototype/getFloat32/return-value-clean-arraybuffer");

    [Fact(DisplayName = "prototype/getFloat32/return-values-custom-offset")]
    public Task prototype_getFloat32_return_values_custom_offset()
        => ExecutionTestFromFile("prototype/getFloat32/return-values-custom-offset");

    [Fact(DisplayName = "prototype/getFloat32/return-values")]
    public Task prototype_getFloat32_return_values()
        => ExecutionTestFromFile("prototype/getFloat32/return-values");

    [Fact(DisplayName = "prototype/getFloat32/this-has-no-dataview-internal")]
    public Task prototype_getFloat32_this_has_no_dataview_internal()
        => ExecutionTestFromFile("prototype/getFloat32/this-has-no-dataview-internal");

    [Fact(DisplayName = "prototype/getFloat32/this-is-not-object")]
    public Task prototype_getFloat32_this_is_not_object()
        => ExecutionTestFromFile("prototype/getFloat32/this-is-not-object");

    [Fact(DisplayName = "prototype/getFloat32/to-boolean-littleendian")]
    public Task prototype_getFloat32_to_boolean_littleendian()
        => ExecutionTestFromFile("prototype/getFloat32/to-boolean-littleendian");

    [Fact(DisplayName = "prototype/getFloat32/toindex-byteoffset")]
    public Task prototype_getFloat32_toindex_byteoffset()
        => ExecutionTestFromFile("prototype/getFloat32/toindex-byteoffset");

    [Fact(DisplayName = "prototype/getFloat64/index-is-out-of-range")]
    public Task prototype_getFloat64_index_is_out_of_range()
        => ExecutionTestFromFile("prototype/getFloat64/index-is-out-of-range");

    [Fact(DisplayName = "prototype/getFloat64/minus-zero")]
    public Task prototype_getFloat64_minus_zero()
        => ExecutionTestFromFile("prototype/getFloat64/minus-zero");

    [Fact(DisplayName = "prototype/getFloat64/negative-byteoffset-throws")]
    public Task prototype_getFloat64_negative_byteoffset_throws()
        => ExecutionTestFromFile("prototype/getFloat64/negative-byteoffset-throws");

    [Fact(DisplayName = "prototype/getFloat64/return-abrupt-from-tonumber-byteoffset-symbol")]
    public Task prototype_getFloat64_return_abrupt_from_tonumber_byteoffset_symbol()
        => ExecutionTestFromFile("prototype/getFloat64/return-abrupt-from-tonumber-byteoffset-symbol");

    [Fact(DisplayName = "prototype/getFloat64/return-abrupt-from-tonumber-byteoffset")]
    public Task prototype_getFloat64_return_abrupt_from_tonumber_byteoffset()
        => ExecutionTestFromFile("prototype/getFloat64/return-abrupt-from-tonumber-byteoffset");

    [Fact(DisplayName = "prototype/getFloat64/return-infinity")]
    public Task prototype_getFloat64_return_infinity()
        => ExecutionTestFromFile("prototype/getFloat64/return-infinity");

    [Fact(DisplayName = "prototype/getFloat64/return-nan")]
    public Task prototype_getFloat64_return_nan()
        => ExecutionTestFromFile("prototype/getFloat64/return-nan");

    [Fact(DisplayName = "prototype/getFloat64/return-value-clean-arraybuffer")]
    public Task prototype_getFloat64_return_value_clean_arraybuffer()
        => ExecutionTestFromFile("prototype/getFloat64/return-value-clean-arraybuffer");

    [Fact(DisplayName = "prototype/getFloat64/return-values-custom-offset")]
    public Task prototype_getFloat64_return_values_custom_offset()
        => ExecutionTestFromFile("prototype/getFloat64/return-values-custom-offset");

    [Fact(DisplayName = "prototype/getFloat64/return-values")]
    public Task prototype_getFloat64_return_values()
        => ExecutionTestFromFile("prototype/getFloat64/return-values");

    [Fact(DisplayName = "prototype/getFloat64/this-has-no-dataview-internal")]
    public Task prototype_getFloat64_this_has_no_dataview_internal()
        => ExecutionTestFromFile("prototype/getFloat64/this-has-no-dataview-internal");

    [Fact(DisplayName = "prototype/getFloat64/this-is-not-object")]
    public Task prototype_getFloat64_this_is_not_object()
        => ExecutionTestFromFile("prototype/getFloat64/this-is-not-object");

    [Fact(DisplayName = "prototype/getFloat64/to-boolean-littleendian")]
    public Task prototype_getFloat64_to_boolean_littleendian()
        => ExecutionTestFromFile("prototype/getFloat64/to-boolean-littleendian");

    [Fact(DisplayName = "prototype/getFloat64/toindex-byteoffset")]
    public Task prototype_getFloat64_toindex_byteoffset()
        => ExecutionTestFromFile("prototype/getFloat64/toindex-byteoffset");

    [Fact(DisplayName = "prototype/getInt16/index-is-out-of-range")]
    public Task prototype_getInt16_index_is_out_of_range()
        => ExecutionTestFromFile("prototype/getInt16/index-is-out-of-range");

    [Fact(DisplayName = "prototype/getInt16/negative-byteoffset-throws")]
    public Task prototype_getInt16_negative_byteoffset_throws()
        => ExecutionTestFromFile("prototype/getInt16/negative-byteoffset-throws");

    [Fact(DisplayName = "prototype/getInt16/return-abrupt-from-tonumber-byteoffset-symbol")]
    public Task prototype_getInt16_return_abrupt_from_tonumber_byteoffset_symbol()
        => ExecutionTestFromFile("prototype/getInt16/return-abrupt-from-tonumber-byteoffset-symbol");

    [Fact(DisplayName = "prototype/getInt16/return-abrupt-from-tonumber-byteoffset")]
    public Task prototype_getInt16_return_abrupt_from_tonumber_byteoffset()
        => ExecutionTestFromFile("prototype/getInt16/return-abrupt-from-tonumber-byteoffset");

    [Fact(DisplayName = "prototype/getInt16/return-value-clean-arraybuffer")]
    public Task prototype_getInt16_return_value_clean_arraybuffer()
        => ExecutionTestFromFile("prototype/getInt16/return-value-clean-arraybuffer");

    [Fact(DisplayName = "prototype/getInt16/return-values-custom-offset")]
    public Task prototype_getInt16_return_values_custom_offset()
        => ExecutionTestFromFile("prototype/getInt16/return-values-custom-offset");

    [Fact(DisplayName = "prototype/getInt16/return-values")]
    public Task prototype_getInt16_return_values()
        => ExecutionTestFromFile("prototype/getInt16/return-values");

    [Fact(DisplayName = "prototype/getInt16/this-has-no-dataview-internal")]
    public Task prototype_getInt16_this_has_no_dataview_internal()
        => ExecutionTestFromFile("prototype/getInt16/this-has-no-dataview-internal");

    [Fact(DisplayName = "prototype/getInt16/this-is-not-object")]
    public Task prototype_getInt16_this_is_not_object()
        => ExecutionTestFromFile("prototype/getInt16/this-is-not-object");

    [Fact(DisplayName = "prototype/getInt16/to-boolean-littleendian")]
    public Task prototype_getInt16_to_boolean_littleendian()
        => ExecutionTestFromFile("prototype/getInt16/to-boolean-littleendian");

    [Fact(DisplayName = "prototype/getInt16/toindex-byteoffset")]
    public Task prototype_getInt16_toindex_byteoffset()
        => ExecutionTestFromFile("prototype/getInt16/toindex-byteoffset");

}
