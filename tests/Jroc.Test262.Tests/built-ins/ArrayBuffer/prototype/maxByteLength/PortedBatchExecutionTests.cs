using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.ArrayBuffer.prototype.maxByteLength;

public class PortedBatchExecutionTests : DiskExecutionTestsBase
{
    public PortedBatchExecutionTests() : base("built_ins.ArrayBuffer.prototype.maxByteLength") { }

    [Fact(DisplayName = "invoked-as-func.js")]
    public Task invoked_as_func() => ExecutionTestFromFile("invoked-as-func");

    [Fact(DisplayName = "return-maxbytelength-resizable.js")]
    public Task return_maxbytelength_resizable() => ExecutionTestFromFile("return-maxbytelength-resizable");

    [Fact(DisplayName = "this-has-no-arraybufferdata-internal.js")]
    public Task this_has_no_arraybufferdata_internal() => ExecutionTestFromFile("this-has-no-arraybufferdata-internal");

    [Fact(DisplayName = "this-is-not-object.js")]
    public Task this_is_not_object() => ExecutionTestFromFile("this-is-not-object");

}
