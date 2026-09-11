using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.defineProperties;

public class FailingBatch100ExecutionTests : DiskExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Object.defineProperties") { }

    [Fact(DisplayName = "15.2.3.7-6-a-292")]
    public Task _15_2_3_7_6_a_292() => ExecutionTestFromFile("15.2.3.7-6-a-292");

    [Fact(DisplayName = "15.2.3.7-6-a-314")]
    public Task _15_2_3_7_6_a_314() => ExecutionTestFromFile("15.2.3.7-6-a-314");

    [Fact(DisplayName = "proxy-no-ownkeys-returned-keys-order")]
    public Task proxy_no_ownkeys_returned_keys_order() => ExecutionTestFromFile("proxy-no-ownkeys-returned-keys-order");

    [Fact(DisplayName = "typedarray-backed-by-resizable-buffer")]
    public Task typedarray_backed_by_resizable_buffer() => ExecutionTestFromFile("typedarray-backed-by-resizable-buffer");

}
