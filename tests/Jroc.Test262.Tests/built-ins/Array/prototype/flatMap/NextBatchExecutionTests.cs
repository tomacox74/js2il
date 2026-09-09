using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.flatMap;

public class NextBatchExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.Array.prototype.flatMap") { }

    [Fact(DisplayName = "proxy-access-count")]
    public Task proxy_access_count() => ExecutionTestFromFile("proxy-access-count");

    [Fact(DisplayName = "target-array-with-non-writable-property")]
    public Task target_array_with_non_writable_property() => ExecutionTestFromFile("target-array-with-non-writable-property");

    [Fact(DisplayName = "this-value-ctor-object-species-custom-ctor")]
    public Task this_value_ctor_object_species_custom_ctor() => ExecutionTestFromFile("this-value-ctor-object-species-custom-ctor");

}
