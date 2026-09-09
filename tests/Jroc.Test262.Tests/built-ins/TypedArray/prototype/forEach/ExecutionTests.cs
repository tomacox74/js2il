using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.forEach;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.TypedArray.prototype.forEach") { }

    [Fact(DisplayName = "callbackfn-detachbuffer")]
    public Task callbackfn_detachbuffer()
        => ExecutionTestFromFile("callbackfn-detachbuffer");

    [Fact(DisplayName = "callbackfn-resize")]
    public Task callbackfn_resize()
        => ExecutionTestFromFile("callbackfn-resize");

    [Fact(DisplayName = "detached-buffer")]
    public Task detached_buffer()
        => ExecutionTestFromFile("detached-buffer");

    [Fact(DisplayName = "invoked-as-func")]
    public Task invoked_as_func()
        => ExecutionTestFromFile("invoked-as-func");

    [Fact(DisplayName = "invoked-as-method")]
    public Task invoked_as_method()
        => ExecutionTestFromFile("invoked-as-method");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "resizable-buffer-grow-mid-iteration")]
    public Task resizable_buffer_grow_mid_iteration()
        => ExecutionTestFromFile("resizable-buffer-grow-mid-iteration");

    [Fact(DisplayName = "resizable-buffer-shrink-mid-iteration")]
    public Task resizable_buffer_shrink_mid_iteration()
        => ExecutionTestFromFile("resizable-buffer-shrink-mid-iteration");

    [Fact(DisplayName = "resizable-buffer")]
    public Task resizable_buffer()
        => ExecutionTestFromFile("resizable-buffer");

    [Fact(DisplayName = "return-abrupt-from-this-out-of-bounds")]
    public Task return_abrupt_from_this_out_of_bounds()
        => ExecutionTestFromFile("return-abrupt-from-this-out-of-bounds");
}
