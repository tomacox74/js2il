using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.ArrayBuffer.prototype.resizable;

public class PortedBatchExecutionTests : DiskExecutionTestsBase
{
    public PortedBatchExecutionTests() : base("built_ins.ArrayBuffer.prototype.resizable") { }

    [Fact(DisplayName = "invoked-as-func.js")]
    public Task invoked_as_func() => ExecutionTestFromFile("invoked-as-func");

    [Fact(DisplayName = "return-resizable.js")]
    public Task return_resizable() => ExecutionTestFromFile("return-resizable");

    [Fact(DisplayName = "this-has-no-arraybufferdata-internal.js")]
    public Task this_has_no_arraybufferdata_internal() => ExecutionTestFromFile("this-has-no-arraybufferdata-internal");

    [Fact(DisplayName = "this-is-not-object.js")]
    public Task this_is_not_object() => ExecutionTestFromFile("this-is-not-object");

}
