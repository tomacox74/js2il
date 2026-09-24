using Jroc.Test262.Tests.language;

namespace Jroc.Test262.Tests.language.block_scope.syntax.for_in;

public class ExecutionTests : DiskExecutionTestsBase
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

    [Fact(DisplayName = "disallow-initialization-assignment.js")]
    public Task disallow_initialization_assignment() => CompilationFailureTest("disallow-initialization-assignment", "Failed to parse JavaScript");

    [Fact(DisplayName = "disallow-multiple-lexical-bindings-with-and-without-initializer.js")]
    public Task disallow_multiple_lexical_bindings_with_and_without_initializer() => CompilationFailureTest("disallow-multiple-lexical-bindings-with-and-without-initializer", "Failed to parse JavaScript");

    [Fact(DisplayName = "disallow-multiple-lexical-bindings-with-initializer.js")]
    public Task disallow_multiple_lexical_bindings_with_initializer() => CompilationFailureTest("disallow-multiple-lexical-bindings-with-initializer", "Failed to parse JavaScript");

    [Fact(DisplayName = "disallow-multiple-lexical-bindings-without-and-with-initializer.js")]
    public Task disallow_multiple_lexical_bindings_without_and_with_initializer() => CompilationFailureTest("disallow-multiple-lexical-bindings-without-and-with-initializer", "Failed to parse JavaScript");

    [Fact(DisplayName = "disallow-multiple-lexical-bindings.js")]
    public Task disallow_multiple_lexical_bindings() => CompilationFailureTest("disallow-multiple-lexical-bindings", "Failed to parse JavaScript");
}
