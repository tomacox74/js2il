using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.getOwnPropertySymbols;

public partial class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.getOwnPropertySymbols") { }

    [Fact(DisplayName = "proxy-invariant-not-extensible-extra-string-key.js")]
    public Task Port200_proxy_invariant_not_extensible_extra_string_key()
        => ExecutionTestFromFile("proxy-invariant-not-extensible-extra-string-key");
}
