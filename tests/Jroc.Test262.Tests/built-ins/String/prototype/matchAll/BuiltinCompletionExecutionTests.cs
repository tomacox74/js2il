using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.matchAll;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.String.prototype.matchAll") { }

    [Fact(DisplayName = "regexp-prototype-matchAll-v-u-flag.js")]
    public Task test_regexp_prototype_matchAll_v_u_flag() => ExecutionTest("regexp-prototype-matchAll-v-u-flag");
}
