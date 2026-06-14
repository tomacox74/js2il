using Jroc.Tests;

namespace Jroc.Test262.Tests.language.block_scope.syntax.for_in;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.block_scope.syntax.for_in") { }

    [Fact(DisplayName = "acquire-properties-from-array")]
    public Task acquire_properties_from_array()
        => ExecutionTest("acquire-properties-from-array");

    [Fact(DisplayName = "acquire-properties-from-object")]
    public Task acquire_properties_from_object()
        => ExecutionTest("acquire-properties-from-object");

    [Fact(DisplayName = "mixed-values-in-iteration")]
    public Task mixed_values_in_iteration()
        => ExecutionTest("mixed-values-in-iteration");
}
