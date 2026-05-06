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

[Fact(DisplayName = "object-pattern-with-no-property-list", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task object_pattern_with_no_property_list()
        => ExecutionTest("object-pattern-with-no-property-list");

[Fact(DisplayName = "property-list-bindings-elements", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task property_list_bindings_elements()
        => ExecutionTest("property-list-bindings-elements");

[Fact(DisplayName = "property-list-followed-by-a-single-comma", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task property_list_followed_by_a_single_comma()
        => ExecutionTest("property-list-followed-by-a-single-comma");

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

[Fact(DisplayName = "object-pattern-with-no-property-list", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task object_pattern_with_no_property_list()
        => ExecutionTest("object-pattern-with-no-property-list");

[Fact(DisplayName = "property-list-bindings-elements", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task property_list_bindings_elements()
        => ExecutionTest("property-list-bindings-elements");

[Fact(DisplayName = "property-list-followed-by-a-single-comma", Skip = "Product gap: currently fails in JS2IL runtime.")]
    public Task property_list_followed_by_a_single_comma()
        => ExecutionTest("property-list-followed-by-a-single-comma");

[Fact(DisplayName = "property-list-single-name-bindings", Skip = "Known issue: runtime behavior diverges from test262 expectation")]
    public Task property_list_single_name_bindings()
        => ExecutionTest("property-list-single-name-bindings");

[Fact(DisplayName = "property-list-with-property-list", Skip = "Known issue: runtime behavior diverges from test262 expectation")]
    public Task property_list_with_property_list()
        => ExecutionTest("property-list-with-property-list");

[Fact(DisplayName = "recursive-array-and-object-patterns", Skip = "Known issue: runtime behavior diverges from test262 expectation")]
    public Task recursive_array_and_object_patterns()
        => ExecutionTest("recursive-array-and-object-patterns");
}
