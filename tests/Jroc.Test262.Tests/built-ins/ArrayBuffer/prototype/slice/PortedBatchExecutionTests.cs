using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.ArrayBuffer.prototype.slice;

public class PortedBatchExecutionTests : DiskExecutionTestsBase
{
    public PortedBatchExecutionTests() : base("built_ins.ArrayBuffer.prototype.slice") { }

    [Fact(DisplayName = "context-is-not-arraybuffer-object.js")]
    public Task context_is_not_arraybuffer_object() => ExecutionTestFromFile("context-is-not-arraybuffer-object");

    [Fact(DisplayName = "context-is-not-object.js")]
    public Task context_is_not_object() => ExecutionTestFromFile("context-is-not-object");

    [Fact(DisplayName = "end-default-if-absent.js")]
    public Task end_default_if_absent() => ExecutionTestFromFile("end-default-if-absent");

    [Fact(DisplayName = "end-default-if-undefined.js")]
    public Task end_default_if_undefined() => ExecutionTestFromFile("end-default-if-undefined");

    [Fact(DisplayName = "end-exceeds-length.js")]
    public Task end_exceeds_length() => ExecutionTestFromFile("end-exceeds-length");

    [Fact(DisplayName = "negative-end.js")]
    public Task negative_end() => ExecutionTestFromFile("negative-end");

    [Fact(DisplayName = "negative-start.js")]
    public Task negative_start() => ExecutionTestFromFile("negative-start");

    [Fact(DisplayName = "nonconstructor.js")]
    public Task nonconstructor() => ExecutionTestFromFile("nonconstructor");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "number-conversion.js")]
    public Task number_conversion() => ExecutionTestFromFile("number-conversion");

    [Fact(DisplayName = "species-constructor-is-undefined.js")]
    public Task species_constructor_is_undefined() => ExecutionTestFromFile("species-constructor-is-undefined");

    [Fact(DisplayName = "species-is-null.js")]
    public Task species_is_null() => ExecutionTestFromFile("species-is-null");

    [Fact(DisplayName = "species-is-undefined.js")]
    public Task species_is_undefined() => ExecutionTestFromFile("species-is-undefined");

    [Fact(DisplayName = "start-default-if-absent.js")]
    public Task start_default_if_absent() => ExecutionTestFromFile("start-default-if-absent");

    [Fact(DisplayName = "start-default-if-undefined.js")]
    public Task start_default_if_undefined() => ExecutionTestFromFile("start-default-if-undefined");

    [Fact(DisplayName = "start-exceeds-end.js")]
    public Task start_exceeds_end() => ExecutionTestFromFile("start-exceeds-end");

    [Fact(DisplayName = "start-exceeds-length.js")]
    public Task start_exceeds_length() => ExecutionTestFromFile("start-exceeds-length");

    [Fact(DisplayName = "this-is-sharedarraybuffer.js")]
    public Task this_is_sharedarraybuffer() => ExecutionTestFromFile("this-is-sharedarraybuffer");

    [Fact(DisplayName = "tointeger-conversion-end.js")]
    public Task tointeger_conversion_end() => ExecutionTestFromFile("tointeger-conversion-end");

    [Fact(DisplayName = "tointeger-conversion-start.js")]
    public Task tointeger_conversion_start() => ExecutionTestFromFile("tointeger-conversion-start");

}
