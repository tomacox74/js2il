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

}
