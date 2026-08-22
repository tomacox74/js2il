using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.ArrayBuffer;

public class PortNext200ExecutionTests : DiskExecutionTestsBase
{
    public PortNext200ExecutionTests() : base("built_ins.ArrayBuffer") { }

    [Fact(DisplayName = "isView/arg-has-no-viewedarraybuffer")]
    public Task isView_arg_has_no_viewedarraybuffer()
        => ExecutionTestFromFile("isView/arg-has-no-viewedarraybuffer");

    [Fact(DisplayName = "isView/arg-is-arraybuffer")]
    public Task isView_arg_is_arraybuffer()
        => ExecutionTestFromFile("isView/arg-is-arraybuffer");

    [Fact(DisplayName = "isView/arg-is-dataview-buffer")]
    public Task isView_arg_is_dataview_buffer()
        => ExecutionTestFromFile("isView/arg-is-dataview-buffer");

    [Fact(DisplayName = "isView/arg-is-dataview-constructor")]
    public Task isView_arg_is_dataview_constructor()
        => ExecutionTestFromFile("isView/arg-is-dataview-constructor");

    [Fact(DisplayName = "isView/arg-is-typedarray")]
    public Task isView_arg_is_typedarray()
        => ExecutionTestFromFile("isView/arg-is-typedarray");

    [Fact(DisplayName = "isView/arg-is-dataview")]
    public Task isView_arg_is_dataview()
        => ExecutionTestFromFile("isView/arg-is-dataview");

    [Fact(DisplayName = "isView/arg-is-not-object")]
    public Task isView_arg_is_not_object()
        => ExecutionTestFromFile("isView/arg-is-not-object");

    [Fact(DisplayName = "isView/no-arg")]
    public Task isView_no_arg()
        => ExecutionTestFromFile("isView/no-arg");

}
