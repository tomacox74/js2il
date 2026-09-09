using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Atomics.store;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.Atomics.store") { }

    [Fact(DisplayName = "non-shared-int-views-throws.js")]
    public Task non_shared_int_views_throws() => ExecutionTestFromFile("non-shared-int-views-throws");

    [Fact(DisplayName = "validate-arraytype-before-index-coercion.js")]
    public Task validate_arraytype_before_index_coercion() => ExecutionTestFromFile("validate-arraytype-before-index-coercion");

    [Fact(DisplayName = "validate-arraytype-before-value-coercion.js")]
    public Task validate_arraytype_before_value_coercion() => ExecutionTestFromFile("validate-arraytype-before-value-coercion");
}
