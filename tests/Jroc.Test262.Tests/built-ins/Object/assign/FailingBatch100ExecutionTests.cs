using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.assign;

public class FailingBatch100ExecutionTests : DiskExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Object.assign") { }

    [Fact(DisplayName = "source-own-prop-desc-missing")]
    public Task source_own_prop_desc_missing() => ExecutionTestFromFile("source-own-prop-desc-missing");

    [Fact(DisplayName = "source-own-prop-error")]
    public Task source_own_prop_error() => ExecutionTestFromFile("source-own-prop-error");

    [Fact(DisplayName = "source-own-prop-keys-error")]
    public Task source_own_prop_keys_error() => ExecutionTestFromFile("source-own-prop-keys-error");

    [Fact(DisplayName = "strings-and-symbol-order-proxy")]
    public Task strings_and_symbol_order_proxy() => ExecutionTestFromFile("strings-and-symbol-order-proxy");

    [Fact(DisplayName = "target-Array")]
    public Task target_Array() => ExecutionTestFromFile("target-Array");

}
