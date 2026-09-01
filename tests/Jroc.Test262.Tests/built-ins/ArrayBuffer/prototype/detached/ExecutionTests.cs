using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.ArrayBuffer.prototype.detached;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.ArrayBuffer.prototype.detached") { }

    [Fact(DisplayName = "detached-buffer-resizable.js")]
    public Task detached_buffer_resizable() => ExecutionTestFromFile("detached-buffer-resizable");

    [Fact(DisplayName = "detached-buffer.js")]
    public Task detached_buffer() => ExecutionTestFromFile("detached-buffer");

    [Fact(DisplayName = "invoked-as-accessor.js")]
    public Task invoked_as_accessor() => ExecutionTestFromFile("invoked-as-accessor");

    [Fact(DisplayName = "invoked-as-func.js")]
    public Task invoked_as_func() => ExecutionTestFromFile("invoked-as-func");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "this-has-no-arraybufferdata-internal.js")]
    public Task this_has_no_arraybufferdata_internal() => ExecutionTestFromFile("this-has-no-arraybufferdata-internal");

    [Fact(DisplayName = "this-is-not-object.js")]
    public Task this_is_not_object() => ExecutionTestFromFile("this-is-not-object");

    [Fact(DisplayName = "this-is-sharedarraybuffer-resizable.js")]
    public Task this_is_sharedarraybuffer_resizable() => ExecutionTestFromFile("this-is-sharedarraybuffer-resizable");

    [Fact(DisplayName = "this-is-sharedarraybuffer.js")]
    public Task this_is_sharedarraybuffer() => ExecutionTestFromFile("this-is-sharedarraybuffer");
}
