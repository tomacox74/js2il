using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Atomics.notify;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.Atomics.notify") { }

    [Fact(DisplayName = "count-symbol-throws.js")]
    public Task count_symbol_throws() => ExecutionTestFromFile("count-symbol-throws");

    [Fact(DisplayName = "non-int32-typedarray-throws.js")]
    public Task non_int32_typedarray_throws() => ExecutionTestFromFile("non-int32-typedarray-throws");

    [Fact(DisplayName = "non-shared-bufferdata-non-shared-int-views-throws.js")]
    public Task non_shared_bufferdata_non_shared_int_views_throws() => ExecutionTestFromFile("non-shared-bufferdata-non-shared-int-views-throws");

    [Fact(DisplayName = "non-shared-int-views.js")]
    public Task non_shared_int_views() => ExecutionTestFromFile("non-shared-int-views");

    [Fact(DisplayName = "not-a-typedarray-throws.js")]
    public Task not_a_typedarray_throws() => ExecutionTestFromFile("not-a-typedarray-throws");

    [Fact(DisplayName = "not-an-object-throws.js")]
    public Task not_an_object_throws() => ExecutionTestFromFile("not-an-object-throws");

    [Fact(DisplayName = "validate-arraytype-before-count-coercion.js")]
    public Task validate_arraytype_before_count_coercion() => ExecutionTestFromFile("validate-arraytype-before-count-coercion");

    [Fact(DisplayName = "validate-arraytype-before-index-coercion.js")]
    public Task validate_arraytype_before_index_coercion() => ExecutionTestFromFile("validate-arraytype-before-index-coercion");
}
