using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.SharedArrayBuffer.prototype.growable;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.SharedArrayBuffer.prototype.growable") { }

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

    [Fact(DisplayName = "this-is-arraybuffer.js")]
    public Task this_is_arraybuffer() => ExecutionTestFromFile("this-is-arraybuffer");

    [Fact(DisplayName = "this-is-not-object.js")]
    public Task this_is_not_object() => ExecutionTestFromFile("this-is-not-object");
}
