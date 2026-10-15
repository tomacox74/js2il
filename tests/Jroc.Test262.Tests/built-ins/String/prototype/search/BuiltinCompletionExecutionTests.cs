using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.String.prototype.search;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.String.prototype.search") { }

    [Fact(DisplayName = "regexp-prototype-search-v-flag.js")]
    public Task test_regexp_prototype_search_v_flag() => ExecutionTest("regexp-prototype-search-v-flag");

    [Fact(DisplayName = "regexp-prototype-search-v-u-flag.js")]
    public Task test_regexp_prototype_search_v_u_flag() => ExecutionTest("regexp-prototype-search-v-u-flag");
}
