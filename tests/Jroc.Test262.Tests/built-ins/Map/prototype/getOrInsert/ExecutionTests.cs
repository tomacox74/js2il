using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Map.prototype.getOrInsert;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Map.prototype.getOrInsert") { }

    [Fact(DisplayName = "append-new-values")]
    public Task append_new_values()
        => ExecutionTestFromFile("append-new-values");

    [Fact(DisplayName = "append-new-values-normalizes-zero-key")]
    public Task append_new_values_normalizes_zero_key()
        => ExecutionTestFromFile("append-new-values-normalizes-zero-key");

    [Fact(DisplayName = "append-value-if-key-is-not-present-different-key-types")]
    public Task append_value_if_key_is_not_present_different_key_types()
        => ExecutionTestFromFile("append-value-if-key-is-not-present-different-key-types");

    [Fact(DisplayName = "returns-value-if-key-is-not-present-different-key-types")]
    public Task returns_value_if_key_is_not_present_different_key_types()
        => ExecutionTestFromFile("returns-value-if-key-is-not-present-different-key-types");

    [Fact(DisplayName = "returns-value-if-key-is-present-different-key-types")]
    public Task returns_value_if_key_is_present_different_key_types()
        => ExecutionTestFromFile("returns-value-if-key-is-present-different-key-types");

    [Fact(DisplayName = "returns-value-normalized-zero-key")]
    public Task returns_value_normalized_zero_key()
        => ExecutionTestFromFile("returns-value-normalized-zero-key");
}

