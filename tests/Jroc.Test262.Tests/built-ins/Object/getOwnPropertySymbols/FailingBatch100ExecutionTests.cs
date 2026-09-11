using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.getOwnPropertySymbols;

public class FailingBatch100ExecutionTests : DiskExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Object.getOwnPropertySymbols") { }

    [Fact(DisplayName = "proxy-invariant-absent-not-configurable-string-key")]
    public Task proxy_invariant_absent_not_configurable_string_key() => ExecutionTestFromFile("proxy-invariant-absent-not-configurable-string-key");

    [Fact(DisplayName = "proxy-invariant-duplicate-string-entry")]
    public Task proxy_invariant_duplicate_string_entry() => ExecutionTestFromFile("proxy-invariant-duplicate-string-entry");

    [Fact(DisplayName = "proxy-invariant-not-extensible-absent-string-key")]
    public Task proxy_invariant_not_extensible_absent_string_key() => ExecutionTestFromFile("proxy-invariant-not-extensible-absent-string-key");

}
