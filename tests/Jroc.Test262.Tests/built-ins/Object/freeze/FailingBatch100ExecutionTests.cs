using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.freeze;

public class FailingBatch100ExecutionTests : DiskExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Object.freeze") { }

    [Fact(DisplayName = "15.2.3.9-2-d-8")]
    public Task _15_2_3_9_2_d_8() => ExecutionTestFromFile("15.2.3.9-2-d-8");

    [Fact(DisplayName = "abrupt-completion")]
    public Task abrupt_completion() => ExecutionTestFromFile("abrupt-completion");

    [Fact(DisplayName = "proxy-with-defineProperty-handler")]
    public Task proxy_with_defineProperty_handler() => ExecutionTestFromFile("proxy-with-defineProperty-handler");

    [Fact(DisplayName = "throws-when-false")]
    public Task throws_when_false() => ExecutionTestFromFile("throws-when-false");

    [Fact(DisplayName = "typedarray-backed-by-resizable-buffer")]
    public Task typedarray_backed_by_resizable_buffer() => ExecutionTestFromFile("typedarray-backed-by-resizable-buffer");

}
