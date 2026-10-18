namespace Jroc.Test262.Tests.language.statements.test_class.subclass;

public sealed class ClassDefinitionConformanceExecutionTests : DiskExecutionTestsBase
{
    public ClassDefinitionConformanceExecutionTests() : base("language.statements.test_class.subclass") { }

    [Fact(DisplayName = "builtins.js")]
    public Task builtins()
        => ExecutionTest("builtins");

    [Fact(DisplayName = "class-definition-null-proto-missing-return-override.js")]
    public Task class_definition_null_proto_missing_return_override()
        => ExecutionTest("class-definition-null-proto-missing-return-override");

    [Fact(DisplayName = "class-definition-null-proto.js")]
    public Task class_definition_null_proto()
        => ExecutionTest("class-definition-null-proto");

    [Fact(DisplayName = "class-definition-parent-proto-null.js")]
    public Task class_definition_parent_proto_null()
        => ExecutionTest("class-definition-parent-proto-null");

    [Fact(DisplayName = "default-constructor-2.js")]
    public Task default_constructor_2()
        => ExecutionTest("default-constructor-2");

    [Fact(DisplayName = "default-constructor-spread-override.js")]
    public Task default_constructor_spread_override()
        => ExecutionTest("default-constructor-spread-override");

    [Fact(DisplayName = "derived-class-return-override-catch-finally-arrow.js")]
    public Task derived_class_return_override_catch_finally_arrow()
        => ExecutionTest("derived-class-return-override-catch-finally-arrow");

    [Fact(DisplayName = "derived-class-return-override-catch-finally.js")]
    public Task derived_class_return_override_catch_finally()
        => ExecutionTest("derived-class-return-override-catch-finally");

    [Fact(DisplayName = "derived-class-return-override-catch-super-arrow.js")]
    public Task derived_class_return_override_catch_super_arrow()
        => ExecutionTest("derived-class-return-override-catch-super-arrow");

    [Fact(DisplayName = "derived-class-return-override-catch-super.js")]
    public Task derived_class_return_override_catch_super()
        => ExecutionTest("derived-class-return-override-catch-super");

    [Fact(DisplayName = "derived-class-return-override-catch.js")]
    public Task derived_class_return_override_catch()
        => ExecutionTest("derived-class-return-override-catch");

    [Fact(DisplayName = "derived-class-return-override-finally-super-arrow.js")]
    public Task derived_class_return_override_finally_super_arrow()
        => ExecutionTest("derived-class-return-override-finally-super-arrow");

    [Fact(DisplayName = "derived-class-return-override-finally-super.js")]
    public Task derived_class_return_override_finally_super()
        => ExecutionTest("derived-class-return-override-finally-super");

    [Fact(DisplayName = "derived-class-return-override-for-of-arrow.js")]
    public Task derived_class_return_override_for_of_arrow()
        => ExecutionTest("derived-class-return-override-for-of-arrow");

    [Fact(DisplayName = "derived-class-return-override-with-boolean.js")]
    public Task derived_class_return_override_with_boolean()
        => ExecutionTest("derived-class-return-override-with-boolean");

    [Fact(DisplayName = "derived-class-return-override-with-empty.js")]
    public Task derived_class_return_override_with_empty()
        => ExecutionTest("derived-class-return-override-with-empty");

    [Fact(DisplayName = "derived-class-return-override-with-null.js")]
    public Task derived_class_return_override_with_null()
        => ExecutionTest("derived-class-return-override-with-null");

    [Fact(DisplayName = "derived-class-return-override-with-string.js")]
    public Task derived_class_return_override_with_string()
        => ExecutionTest("derived-class-return-override-with-string");

    [Fact(DisplayName = "derived-class-return-override-with-symbol.js")]
    public Task derived_class_return_override_with_symbol()
        => ExecutionTest("derived-class-return-override-with-symbol");

    [Fact(DisplayName = "superclass-arrow-function.js")]
    public Task superclass_arrow_function()
        => ExecutionTest("superclass-arrow-function");

    [Fact(DisplayName = "superclass-async-function.js")]
    public Task superclass_async_function()
        => ExecutionTest("superclass-async-function");

    [Fact(DisplayName = "superclass-async-generator-function.js")]
    public Task superclass_async_generator_function()
        => ExecutionTest("superclass-async-generator-function");

    [Fact(DisplayName = "superclass-generator-function.js")]
    public Task superclass_generator_function()
        => ExecutionTest("superclass-generator-function");

    [Fact(DisplayName = "superclass-static-method-override.js")]
    public Task superclass_static_method_override()
        => ExecutionTest("superclass-static-method-override");
}
