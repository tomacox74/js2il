using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.internals.DefineOwnProperty;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.TypedArrayConstructors.internals.DefineOwnProperty") { }

    [Fact(DisplayName = "conversion-operation-consistent-nan")]
    public Task conversion_operation_consistent_nan()
        => ExecutionTestFromFile("conversion-operation-consistent-nan");

    [Fact(DisplayName = "conversion-operation")]
    public Task conversion_operation()
        => ExecutionTestFromFile("conversion-operation");

    [Fact(DisplayName = "desc-value-throws")]
    public Task desc_value_throws()
        => ExecutionTestFromFile("desc-value-throws");

    [Fact(DisplayName = "key-is-greater-than-last-index")]
    public Task key_is_greater_than_last_index()
        => ExecutionTestFromFile("key-is-greater-than-last-index");

    [Fact(DisplayName = "key-is-numericindex-desc-configurable")]
    public Task key_is_numericindex_desc_configurable()
        => ExecutionTestFromFile("key-is-numericindex-desc-configurable");

    [Fact(DisplayName = "key-is-numericindex-desc-not-configurable-throws")]
    public Task key_is_numericindex_desc_not_configurable_throws()
        => ExecutionTestFromFile("key-is-numericindex-desc-not-configurable-throws");

    [Fact(DisplayName = "key-is-numericindex")]
    public Task key_is_numericindex()
        => ExecutionTestFromFile("key-is-numericindex");

    [Fact(DisplayName = "set-value")]
    public Task set_value()
        => ExecutionTestFromFile("set-value");

}
