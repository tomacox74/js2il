using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.block_scope.syntax.for_in;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.block_scope.syntax.for_in") { }

[Fact(DisplayName = "acquire-properties-from-array", Skip = "Block-scoped for-in property acquisition from arrays is incomplete.")]
    public Task acquire_properties_from_array()
        => ExecutionTest("acquire-properties-from-array");

[Fact(DisplayName = "acquire-properties-from-object")]
    public Task acquire_properties_from_object()
        => ExecutionTest("acquire-properties-from-object");
}
