using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView;

public class DataViewConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public DataViewConformanceBatchExecutionTests() : base("built_ins.DataView") { }

    [Fact(DisplayName = "buffer-does-not-have-arraybuffer-data-throws-sab.js")]
    public Task buffer_does_not_have_arraybuffer_data_throws_sab() => ExecutionTestFromFile("buffer-does-not-have-arraybuffer-data-throws-sab");

    [Fact(DisplayName = "buffer-reference-sab.js")]
    public Task buffer_reference_sab() => ExecutionTestFromFile("buffer-reference-sab");

    [Fact(DisplayName = "byteoffset-is-negative-throws-sab.js")]
    public Task byteoffset_is_negative_throws_sab() => ExecutionTestFromFile("byteoffset-is-negative-throws-sab");

    [Fact(DisplayName = "custom-proto-access-resizes-buffer-valid-by-length.js")]
    public Task custom_proto_access_resizes_buffer_valid_by_length() => ExecutionTestFromFile("custom-proto-access-resizes-buffer-valid-by-length");

    [Fact(DisplayName = "custom-proto-access-resizes-buffer-valid-by-offset.js")]
    public Task custom_proto_access_resizes_buffer_valid_by_offset() => ExecutionTestFromFile("custom-proto-access-resizes-buffer-valid-by-offset");

    [Fact(DisplayName = "custom-proto-access-throws-sab.js")]
    public Task custom_proto_access_throws_sab() => ExecutionTestFromFile("custom-proto-access-throws-sab");

    [Fact(DisplayName = "custom-proto-access-throws.js")]
    public Task custom_proto_access_throws() => ExecutionTestFromFile("custom-proto-access-throws");

    [Fact(DisplayName = "custom-proto-if-not-object-fallbacks-to-default-prototype-sab.js")]
    public Task custom_proto_if_not_object_fallbacks_to_default_prototype_sab() => ExecutionTestFromFile("custom-proto-if-not-object-fallbacks-to-default-prototype-sab");

    [Fact(DisplayName = "custom-proto-if-object-is-used-sab.js")]
    public Task custom_proto_if_object_is_used_sab() => ExecutionTestFromFile("custom-proto-if-object-is-used-sab");

    [Fact(DisplayName = "custom-proto-if-object-is-used.js")]
    public Task custom_proto_if_object_is_used() => ExecutionTestFromFile("custom-proto-if-object-is-used");

    [Fact(DisplayName = "dataview.js")]
    public Task dataview() => ExecutionTestFromFile("dataview");

    [Fact(DisplayName = "defined-bytelength-and-byteoffset-sab.js")]
    public Task defined_bytelength_and_byteoffset_sab() => ExecutionTestFromFile("defined-bytelength-and-byteoffset-sab");

    [Fact(DisplayName = "defined-byteoffset-sab.js")]
    public Task defined_byteoffset_sab() => ExecutionTestFromFile("defined-byteoffset-sab");

    [Fact(DisplayName = "defined-byteoffset-undefined-bytelength-sab.js")]
    public Task defined_byteoffset_undefined_bytelength_sab() => ExecutionTestFromFile("defined-byteoffset-undefined-bytelength-sab");

    [Fact(DisplayName = "excessive-bytelength-throws-sab.js")]
    public Task excessive_bytelength_throws_sab() => ExecutionTestFromFile("excessive-bytelength-throws-sab");

    [Fact(DisplayName = "excessive-byteoffset-throws-sab.js")]
    public Task excessive_byteoffset_throws_sab() => ExecutionTestFromFile("excessive-byteoffset-throws-sab");

    [Fact(DisplayName = "instance-extensibility-sab.js")]
    public Task instance_extensibility_sab() => ExecutionTestFromFile("instance-extensibility-sab");

    [Fact(DisplayName = "instance-extensibility.js")]
    public Task instance_extensibility() => ExecutionTestFromFile("instance-extensibility");

    [Fact(DisplayName = "negative-bytelength-throws-sab.js")]
    public Task negative_bytelength_throws_sab() => ExecutionTestFromFile("negative-bytelength-throws-sab");

    [Fact(DisplayName = "negative-byteoffset-throws-sab.js")]
    public Task negative_byteoffset_throws_sab() => ExecutionTestFromFile("negative-byteoffset-throws-sab");

    [Fact(DisplayName = "newtarget-undefined-throws-sab.js")]
    public Task newtarget_undefined_throws_sab() => ExecutionTestFromFile("newtarget-undefined-throws-sab");

    [Fact(DisplayName = "prototype.js")]
    public Task prototype() => ExecutionTestFromFile("prototype");

    [Fact(DisplayName = "return-abrupt-tonumber-bytelength-sab.js")]
    public Task return_abrupt_tonumber_bytelength_sab() => ExecutionTestFromFile("return-abrupt-tonumber-bytelength-sab");

    [Fact(DisplayName = "return-abrupt-tonumber-bytelength-symbol-sab.js")]
    public Task return_abrupt_tonumber_bytelength_symbol_sab() => ExecutionTestFromFile("return-abrupt-tonumber-bytelength-symbol-sab");

    [Fact(DisplayName = "return-abrupt-tonumber-bytelength-symbol.js")]
    public Task return_abrupt_tonumber_bytelength_symbol() => ExecutionTestFromFile("return-abrupt-tonumber-bytelength-symbol");

    [Fact(DisplayName = "return-abrupt-tonumber-bytelength.js")]
    public Task return_abrupt_tonumber_bytelength() => ExecutionTestFromFile("return-abrupt-tonumber-bytelength");

    [Fact(DisplayName = "return-abrupt-tonumber-byteoffset-sab.js")]
    public Task return_abrupt_tonumber_byteoffset_sab() => ExecutionTestFromFile("return-abrupt-tonumber-byteoffset-sab");

    [Fact(DisplayName = "return-abrupt-tonumber-byteoffset-symbol-sab.js")]
    public Task return_abrupt_tonumber_byteoffset_symbol_sab() => ExecutionTestFromFile("return-abrupt-tonumber-byteoffset-symbol-sab");

    [Fact(DisplayName = "return-abrupt-tonumber-byteoffset-symbol.js")]
    public Task return_abrupt_tonumber_byteoffset_symbol() => ExecutionTestFromFile("return-abrupt-tonumber-byteoffset-symbol");

    [Fact(DisplayName = "return-abrupt-tonumber-byteoffset.js")]
    public Task return_abrupt_tonumber_byteoffset() => ExecutionTestFromFile("return-abrupt-tonumber-byteoffset");

    [Fact(DisplayName = "return-instance-sab.js")]
    public Task return_instance_sab() => ExecutionTestFromFile("return-instance-sab");

    [Fact(DisplayName = "return-instance.js")]
    public Task return_instance() => ExecutionTestFromFile("return-instance");

    [Fact(DisplayName = "toindex-byteoffset-sab.js")]
    public Task toindex_byteoffset_sab() => ExecutionTestFromFile("toindex-byteoffset-sab");

    [Fact(DisplayName = "toindex-byteoffset.js")]
    public Task toindex_byteoffset() => ExecutionTestFromFile("toindex-byteoffset");

}
