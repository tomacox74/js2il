using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.getOwnPropertyNames;

public class FailingBatch100ExecutionTests : DiskExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Object.getOwnPropertyNames") { }

    [Fact(DisplayName = "proxy-invariant-absent-not-configurable-symbol-key")]
    public Task proxy_invariant_absent_not_configurable_symbol_key() => ExecutionTestFromFile("proxy-invariant-absent-not-configurable-symbol-key");

    [Fact(DisplayName = "proxy-invariant-duplicate-symbol-entry")]
    public Task proxy_invariant_duplicate_symbol_entry() => ExecutionTestFromFile("proxy-invariant-duplicate-symbol-entry");

    [Fact(DisplayName = "proxy-invariant-not-extensible-absent-symbol-key")]
    public Task proxy_invariant_not_extensible_absent_symbol_key() => ExecutionTestFromFile("proxy-invariant-not-extensible-absent-symbol-key");

    [Fact(DisplayName = "proxy-invariant-not-extensible-extra-symbol-key")]
    public Task proxy_invariant_not_extensible_extra_symbol_key() => ExecutionTestFromFile("proxy-invariant-not-extensible-extra-symbol-key");

}
