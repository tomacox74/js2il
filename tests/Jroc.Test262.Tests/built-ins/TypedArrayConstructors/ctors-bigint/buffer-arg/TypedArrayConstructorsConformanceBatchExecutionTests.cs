using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.ctors_bigint.buffer_arg;

public class TypedArrayConstructorsConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConstructorsConformanceBatchExecutionTests() : base("built_ins.TypedArrayConstructors.ctors_bigint.buffer_arg") { }

    [Fact(DisplayName = "bufferbyteoffset-throws-from-modulo-element-size-sab.js")]
    public Task bufferbyteoffset_throws_from_modulo_element_size_sab() => ExecutionTestFromFile("bufferbyteoffset-throws-from-modulo-element-size-sab");

    [Fact(DisplayName = "bufferbyteoffset-throws-from-modulo-element-size.js")]
    public Task bufferbyteoffset_throws_from_modulo_element_size() => ExecutionTestFromFile("bufferbyteoffset-throws-from-modulo-element-size");

    [Fact(DisplayName = "byteoffset-is-negative-throws-sab.js")]
    public Task byteoffset_is_negative_throws_sab() => ExecutionTestFromFile("byteoffset-is-negative-throws-sab");

    [Fact(DisplayName = "byteoffset-is-negative-throws.js")]
    public Task byteoffset_is_negative_throws() => ExecutionTestFromFile("byteoffset-is-negative-throws");

    [Fact(DisplayName = "byteoffset-is-negative-zero-sab.js")]
    public Task byteoffset_is_negative_zero_sab() => ExecutionTestFromFile("byteoffset-is-negative-zero-sab");

    [Fact(DisplayName = "byteoffset-is-negative-zero.js")]
    public Task byteoffset_is_negative_zero() => ExecutionTestFromFile("byteoffset-is-negative-zero");

    [Fact(DisplayName = "byteoffset-is-symbol-throws-sab.js")]
    public Task byteoffset_is_symbol_throws_sab() => ExecutionTestFromFile("byteoffset-is-symbol-throws-sab");

    [Fact(DisplayName = "byteoffset-is-symbol-throws.js")]
    public Task byteoffset_is_symbol_throws() => ExecutionTestFromFile("byteoffset-is-symbol-throws");

    [Fact(DisplayName = "byteoffset-throws-from-modulo-element-size-sab.js")]
    public Task byteoffset_throws_from_modulo_element_size_sab() => ExecutionTestFromFile("byteoffset-throws-from-modulo-element-size-sab");

    [Fact(DisplayName = "byteoffset-throws-from-modulo-element-size.js")]
    public Task byteoffset_throws_from_modulo_element_size() => ExecutionTestFromFile("byteoffset-throws-from-modulo-element-size");

    [Fact(DisplayName = "byteoffset-to-number-throws-sab.js")]
    public Task byteoffset_to_number_throws_sab() => ExecutionTestFromFile("byteoffset-to-number-throws-sab");

    [Fact(DisplayName = "byteoffset-to-number-throws.js")]
    public Task byteoffset_to_number_throws() => ExecutionTestFromFile("byteoffset-to-number-throws");

    [Fact(DisplayName = "custom-proto-access-throws-sab.js")]
    public Task custom_proto_access_throws_sab() => ExecutionTestFromFile("custom-proto-access-throws-sab");

    [Fact(DisplayName = "custom-proto-access-throws.js")]
    public Task custom_proto_access_throws() => ExecutionTestFromFile("custom-proto-access-throws");

    [Fact(DisplayName = "defined-length-and-offset-sab.js")]
    public Task defined_length_and_offset_sab() => ExecutionTestFromFile("defined-length-and-offset-sab");

    [Fact(DisplayName = "defined-length-and-offset.js")]
    public Task defined_length_and_offset() => ExecutionTestFromFile("defined-length-and-offset");

    [Fact(DisplayName = "defined-length-sab.js")]
    public Task defined_length_sab() => ExecutionTestFromFile("defined-length-sab");

    [Fact(DisplayName = "defined-length.js")]
    public Task defined_length() => ExecutionTestFromFile("defined-length");

    [Fact(DisplayName = "defined-negative-length-sab.js")]
    public Task defined_negative_length_sab() => ExecutionTestFromFile("defined-negative-length-sab");

    [Fact(DisplayName = "defined-negative-length.js")]
    public Task defined_negative_length() => ExecutionTestFromFile("defined-negative-length");

    [Fact(DisplayName = "defined-offset-sab.js")]
    public Task defined_offset_sab() => ExecutionTestFromFile("defined-offset-sab");

    [Fact(DisplayName = "defined-offset.js")]
    public Task defined_offset() => ExecutionTestFromFile("defined-offset");

    [Fact(DisplayName = "excessive-length-throws-sab.js")]
    public Task excessive_length_throws_sab() => ExecutionTestFromFile("excessive-length-throws-sab");

    [Fact(DisplayName = "excessive-length-throws.js")]
    public Task excessive_length_throws() => ExecutionTestFromFile("excessive-length-throws");

    [Fact(DisplayName = "excessive-offset-throws-sab.js")]
    public Task excessive_offset_throws_sab() => ExecutionTestFromFile("excessive-offset-throws-sab");

    [Fact(DisplayName = "excessive-offset-throws.js")]
    public Task excessive_offset_throws() => ExecutionTestFromFile("excessive-offset-throws");

    [Fact(DisplayName = "is-referenced-sab.js")]
    public Task is_referenced_sab() => ExecutionTestFromFile("is-referenced-sab");

    [Fact(DisplayName = "is-referenced.js")]
    public Task is_referenced() => ExecutionTestFromFile("is-referenced");

    [Fact(DisplayName = "length-access-throws-sab.js")]
    public Task length_access_throws_sab() => ExecutionTestFromFile("length-access-throws-sab");

    [Fact(DisplayName = "length-access-throws.js")]
    public Task length_access_throws() => ExecutionTestFromFile("length-access-throws");

    [Fact(DisplayName = "length-is-symbol-throws-sab.js")]
    public Task length_is_symbol_throws_sab() => ExecutionTestFromFile("length-is-symbol-throws-sab");

    [Fact(DisplayName = "length-is-symbol-throws.js")]
    public Task length_is_symbol_throws() => ExecutionTestFromFile("length-is-symbol-throws");

    [Fact(DisplayName = "new-instance-extensibility-sab.js")]
    public Task new_instance_extensibility_sab() => ExecutionTestFromFile("new-instance-extensibility-sab");

    [Fact(DisplayName = "new-instance-extensibility.js")]
    public Task new_instance_extensibility() => ExecutionTestFromFile("new-instance-extensibility");

    [Fact(DisplayName = "returns-new-instance-sab.js")]
    public Task returns_new_instance_sab() => ExecutionTestFromFile("returns-new-instance-sab");

    [Fact(DisplayName = "returns-new-instance.js")]
    public Task returns_new_instance() => ExecutionTestFromFile("returns-new-instance");

    [Fact(DisplayName = "typedarray-backed-by-sharedarraybuffer.js")]
    public Task typedarray_backed_by_sharedarraybuffer() => ExecutionTestFromFile("typedarray-backed-by-sharedarraybuffer");

    [Fact(DisplayName = "use-custom-proto-if-object-sab.js")]
    public Task use_custom_proto_if_object_sab() => ExecutionTestFromFile("use-custom-proto-if-object-sab");

    [Fact(DisplayName = "use-custom-proto-if-object.js")]
    public Task use_custom_proto_if_object() => ExecutionTestFromFile("use-custom-proto-if-object");

    [Fact(DisplayName = "use-default-proto-if-custom-proto-is-not-object-sab.js")]
    public Task use_default_proto_if_custom_proto_is_not_object_sab() => ExecutionTestFromFile("use-default-proto-if-custom-proto-is-not-object-sab");

    [Fact(DisplayName = "use-default-proto-if-custom-proto-is-not-object.js")]
    public Task use_default_proto_if_custom_proto_is_not_object() => ExecutionTestFromFile("use-default-proto-if-custom-proto-is-not-object");

}
