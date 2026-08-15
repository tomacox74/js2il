using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.SharedArrayBuffer.prototype.maxByteLength;

public sealed class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.SharedArrayBuffer.prototype.maxByteLength") { }

    [Fact(DisplayName = "invoked-as-accessor.js")]
    public Task invoked_as_accessor() => ExecutionTest("invoked-as-accessor");

    [Fact(DisplayName = "invoked-as-func.js")]
    public Task invoked_as_func() => ExecutionTest("invoked-as-func");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTest("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTest("name");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTest("prop-desc");

    [Fact(DisplayName = "return-maxbytelength-non-growable.js")]
    public Task return_maxbytelength_non_growable() => ExecutionTest("return-maxbytelength-non-growable");

    [Fact(DisplayName = "this-is-arraybuffer.js")]
    public Task this_is_arraybuffer() => ExecutionTest("this-is-arraybuffer");

    [Fact(DisplayName = "this-is-not-object.js")]
    public Task this_is_not_object() => ExecutionTest("this-is-not-object");
}
