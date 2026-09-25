using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.SharedArrayBuffer.prototype.grow;

public sealed class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.SharedArrayBuffer.prototype.grow") { }

    [Fact(DisplayName = "this-is-sharedarraybuffer")]
    public Task this_is_sharedarraybuffer()
        => ExecutionTest("this-is-sharedarraybuffer");

    [Fact(DisplayName = "descriptor.js")]
    public Task ported_descriptor() => ExecutionTest("descriptor");

    [Fact(DisplayName = "extensible.js")]
    public Task ported_extensible() => ExecutionTest("extensible");

    [Fact(DisplayName = "grow-larger-size.js")]
    public Task ported_grow_larger_size() => ExecutionTest("grow-larger-size");

    [Fact(DisplayName = "grow-same-size.js")]
    public Task ported_grow_same_size() => ExecutionTest("grow-same-size");

    [Fact(DisplayName = "grow-smaller-size.js")]
    public Task ported_grow_smaller_size() => ExecutionTest("grow-smaller-size");

    [Fact(DisplayName = "length.js")]
    public Task ported_length() => ExecutionTest("length");

    [Fact(DisplayName = "name.js")]
    public Task ported_name() => ExecutionTest("name");

    [Fact(DisplayName = "new-length-excessive.js")]
    public Task ported_new_length_excessive() => ExecutionTest("new-length-excessive");

    [Fact(DisplayName = "new-length-negative.js")]
    public Task ported_new_length_negative() => ExecutionTest("new-length-negative");

    [Fact(DisplayName = "new-length-non-number.js")]
    public Task ported_new_length_non_number() => ExecutionTest("new-length-non-number");

    [Fact(DisplayName = "this-is-not-arraybuffer-object.js")]
    public Task ported_this_is_not_arraybuffer_object() => ExecutionTest("this-is-not-arraybuffer-object");

    [Fact(DisplayName = "this-is-not-object.js")]
    public Task ported_this_is_not_object() => ExecutionTest("this-is-not-object");

    [Fact(DisplayName = "this-is-not-resizable-arraybuffer-object.js")]
    public Task ported_this_is_not_resizable_arraybuffer_object() => ExecutionTest("this-is-not-resizable-arraybuffer-object");
}
