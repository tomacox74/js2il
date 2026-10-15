using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.concat;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.Array.prototype.concat") { }

    [Fact(DisplayName = "is-concat-spreadable-is-array-proxy-revoked.js")]
    public Task test_is_concat_spreadable_is_array_proxy_revoked() => ExecutionTest("is-concat-spreadable-is-array-proxy-revoked");
}
