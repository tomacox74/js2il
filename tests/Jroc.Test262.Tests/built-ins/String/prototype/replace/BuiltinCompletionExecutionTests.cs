using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.replace;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.String.prototype.replace") { }

    [Fact(DisplayName = "regexp-prototype-replace-v-u-flag.js")]
    public Task test_regexp_prototype_replace_v_u_flag() => ExecutionTest("regexp-prototype-replace-v-u-flag");
}
