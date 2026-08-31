namespace Jroc.Test262.Tests.built_ins.Array.prototype.flatMap;

public partial class ExecutionTests
{
    [Fact(DisplayName = "array-like-objects-nested")]
    public Task array_like_objects_nested() => ExecutionTestFromFile("array-like-objects-nested");
    [Fact(DisplayName = "array-like-objects-poisoned-length")]
    public Task array_like_objects_poisoned_length() => ExecutionTestFromFile("array-like-objects-poisoned-length");
    [Fact(DisplayName = "array-like-objects-typedarrays")]
    public Task array_like_objects_typedarrays() => ExecutionTestFromFile("array-like-objects-typedarrays");
    [Fact(DisplayName = "bound-function-argument")]
    public Task bound_function_argument() => ExecutionTestFromFile("bound-function-argument");
    [Fact(DisplayName = "depth-always-one")]
    public Task depth_always_one() => ExecutionTestFromFile("depth-always-one");
    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");
    [Fact(DisplayName = "mapperfunction-throws")]
    public Task mapperfunction_throws() => ExecutionTestFromFile("mapperfunction-throws");
    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");
    [Fact(DisplayName = "non-callable-argument-throws")]
    public Task non_callable_argument_throws() => ExecutionTestFromFile("non-callable-argument-throws");
    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");
    [Fact(DisplayName = "target-array-non-extensible")]
    public Task target_array_non_extensible() => ExecutionTestFromFile("target-array-non-extensible");
    [Fact(DisplayName = "target-array-with-non-configurable-property")]
    public Task target_array_with_non_configurable_property() => ExecutionTestFromFile("target-array-with-non-configurable-property");
    [Fact(DisplayName = "this-value-ctor-non-object")]
    public Task this_value_ctor_non_object() => ExecutionTestFromFile("this-value-ctor-non-object");
    [Fact(DisplayName = "this-value-ctor-object-species-bad-throws")]
    public Task this_value_ctor_object_species_bad_throws() => ExecutionTestFromFile("this-value-ctor-object-species-bad-throws");
    [Fact(DisplayName = "this-value-ctor-object-species-custom-ctor-poisoned-throws")]
    public Task this_value_ctor_object_species_custom_ctor_poisoned_throws() => ExecutionTestFromFile("this-value-ctor-object-species-custom-ctor-poisoned-throws");
    [Fact(DisplayName = "this-value-ctor-object-species")]
    public Task this_value_ctor_object_species() => ExecutionTestFromFile("this-value-ctor-object-species");
    [Fact(DisplayName = "this-value-null-undefined-throws")]
    public Task this_value_null_undefined_throws() => ExecutionTestFromFile("this-value-null-undefined-throws");
}
