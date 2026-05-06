using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.destructuring.binding.syntax;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.destructuring.binding.syntax") { }

[Fact(DisplayName = "destructuring-array-parameters-function-arguments-length")]
    public Task destructuring_array_parameters_function_arguments_length()
        => ExecutionTest("destructuring-array-parameters-function-arguments-length");

[Fact(DisplayName = "destructuring-object-parameters-function-arguments-length")]
    public Task destructuring_object_parameters_function_arguments_length()
        => ExecutionTest("destructuring-object-parameters-function-arguments-length");

[Fact(DisplayName = "array-elements-with-initializer")]
    public Task array_elements_with_initializer()
        => ExecutionTest("array-elements-with-initializer");

[Fact(DisplayName = "array-elements-with-object-patterns")]
    public Task array_elements_with_object_patterns()
        => ExecutionTest("array-elements-with-object-patterns");

[Fact(DisplayName = "array-elements-without-initializer")]
    public Task array_elements_without_initializer()
        => ExecutionTest("array-elements-without-initializer");

[Fact(DisplayName = "array-pattern-with-elisions")]
    public Task array_pattern_with_elisions()
        => ExecutionTest("array-pattern-with-elisions");

[Fact(DisplayName = "array-pattern-with-no-elements")]
    public Task array_pattern_with_no_elements()
        => ExecutionTest("array-pattern-with-no-elements");

[Fact(DisplayName = "array-rest-elements")]
    public Task array_rest_elements()
        => ExecutionTest("array-rest-elements");
}
