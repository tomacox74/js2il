using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.seal;

public class FailingBatch100ExecutionTests : DiskExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Object.seal") { }

    [Fact(DisplayName = "abrupt-completion")]
    public Task abrupt_completion() => ExecutionTestFromFile("abrupt-completion");

    [Fact(DisplayName = "object-seal-o-is-an-error-object")]
    public Task object_seal_o_is_an_error_object() => ExecutionTestFromFile("object-seal-o-is-an-error-object");

    [Fact(DisplayName = "proxy-no-ownkeys-returned-keys-order")]
    public Task proxy_no_ownkeys_returned_keys_order() => ExecutionTestFromFile("proxy-no-ownkeys-returned-keys-order");

    [Fact(DisplayName = "proxy-with-defineProperty-handler")]
    public Task proxy_with_defineProperty_handler() => ExecutionTestFromFile("proxy-with-defineProperty-handler");

    [Fact(DisplayName = "seal-arrowfunction")]
    public Task seal_arrowfunction() => ExecutionTestFromFile("seal-arrowfunction");

    [Fact(DisplayName = "throws-when-false")]
    public Task throws_when_false() => ExecutionTestFromFile("throws-when-false");

}
