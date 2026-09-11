using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Proxy.ownKeys;

public class FailingBatch100ExecutionTests : DiskExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Proxy.ownKeys") { }

    [Fact(DisplayName = "not-extensible-missing-keys-throws")]
    public Task not_extensible_missing_keys_throws() => ExecutionTestFromFile("not-extensible-missing-keys-throws");

    [Fact(DisplayName = "not-extensible-new-keys-throws")]
    public Task not_extensible_new_keys_throws() => ExecutionTestFromFile("not-extensible-new-keys-throws");

    [Fact(DisplayName = "return-all-non-configurable-keys")]
    public Task return_all_non_configurable_keys() => ExecutionTestFromFile("return-all-non-configurable-keys");

    [Fact(DisplayName = "return-duplicate-entries-throws")]
    public Task return_duplicate_entries_throws() => ExecutionTestFromFile("return-duplicate-entries-throws");

    [Fact(DisplayName = "return-duplicate-symbol-entries-throws")]
    public Task return_duplicate_symbol_entries_throws() => ExecutionTestFromFile("return-duplicate-symbol-entries-throws");

}
