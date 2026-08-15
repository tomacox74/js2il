using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.SharedArrayBuffer.prototype.byteLength;

public sealed class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.SharedArrayBuffer.prototype.byteLength") { }

    [Fact(DisplayName = "return-bytelength")]
    public Task return_bytelength()
        => ExecutionTest("return-bytelength");

    [Fact(DisplayName = "invoked-as-accessor.js")]
    public Task invoked_as_accessor()
        => ExecutionTest("invoked-as-accessor");

    [Fact(DisplayName = "invoked-as-func.js")]
    public Task invoked_as_func()
        => ExecutionTest("invoked-as-func");

    [Fact(DisplayName = "length.js")]
    public Task length()
        => ExecutionTest("length");

    [Fact(DisplayName = "name.js")]
    public Task name()
        => ExecutionTest("name");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc()
        => ExecutionTest("prop-desc");

    [Fact(DisplayName = "this-has-no-typedarrayname-internal.js")]
    public Task this_has_no_typedarrayname_internal()
        => ExecutionTest("this-has-no-typedarrayname-internal");

    [Fact(DisplayName = "this-is-arraybuffer.js")]
    public Task this_is_arraybuffer()
        => ExecutionTest("this-is-arraybuffer");

    [Fact(DisplayName = "this-is-not-object.js")]
    public Task this_is_not_object()
        => ExecutionTest("this-is-not-object");
}
