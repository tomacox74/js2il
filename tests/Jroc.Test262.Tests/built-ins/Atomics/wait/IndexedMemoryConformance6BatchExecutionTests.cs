using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Atomics.wait;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.Atomics.wait") { }

    [Fact(DisplayName = "descriptor.js")]
    public Task descriptor() => ExecutionTestFromFile("descriptor");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "non-int32-typedarray-throws.js")]
    public Task non_int32_typedarray_throws() => ExecutionTestFromFile("non-int32-typedarray-throws");

    [Fact(DisplayName = "poisoned-object-for-timeout-throws.js")]
    public Task poisoned_object_for_timeout_throws() => ExecutionTestFromFile("poisoned-object-for-timeout-throws");

    [Fact(DisplayName = "symbol-for-value-throws.js")]
    public Task symbol_for_value_throws() => ExecutionTestFromFile("symbol-for-value-throws");

    [Fact(DisplayName = "validate-arraytype-before-index-coercion.js")]
    public Task validate_arraytype_before_index_coercion() => ExecutionTestFromFile("validate-arraytype-before-index-coercion");

    [Fact(DisplayName = "validate-arraytype-before-timeout-coercion.js")]
    public Task validate_arraytype_before_timeout_coercion() => ExecutionTestFromFile("validate-arraytype-before-timeout-coercion");

    [Fact(DisplayName = "validate-arraytype-before-value-coercion.js")]
    public Task validate_arraytype_before_value_coercion() => ExecutionTestFromFile("validate-arraytype-before-value-coercion");
}
