using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.flat;

public class NextBatchExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.Array.prototype.flat") { }

    [Fact(DisplayName = "non-object-ctor-throws")]
    public Task non_object_ctor_throws() => ExecutionTestFromFile("non-object-ctor-throws");

    [Fact(DisplayName = "proxy-access-count")]
    public Task proxy_access_count() => ExecutionTestFromFile("proxy-access-count");

    [Fact(DisplayName = "symbol-object-create-null-depth-throws")]
    public Task symbol_object_create_null_depth_throws() => ExecutionTestFromFile("symbol-object-create-null-depth-throws");

    [Fact(DisplayName = "target-array-non-extensible")]
    public Task target_array_non_extensible() => ExecutionTestFromFile("target-array-non-extensible");

    [Fact(DisplayName = "target-array-with-non-configurable-property")]
    public Task target_array_with_non_configurable_property() => ExecutionTestFromFile("target-array-with-non-configurable-property");

}
