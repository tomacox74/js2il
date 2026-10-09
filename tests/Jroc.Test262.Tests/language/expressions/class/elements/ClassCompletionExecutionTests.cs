using Jroc.Test262.Tests;

namespace Jroc.Test262.Tests.language.expressions.Test_class.elements;

public sealed class ClassCompletionExecutionTests : DiskExecutionTestsBase
{
    public ClassCompletionExecutionTests() : base("Jroc.Test262.Tests.language.expressions.Test_class.elements") { }

    [Fact(DisplayName = "arrow-body-derived-cls-direct-eval-contains-superproperty-1.js", Skip = "eval is not supported.")]
    public Task arrow_body_derived_cls_direct_eval_contains_superproperty_1()
        => ExecutionTestFromFile("arrow-body-derived-cls-direct-eval-contains-superproperty-1");

    [Fact(DisplayName = "arrow-body-derived-cls-direct-eval-contains-superproperty-2.js", Skip = "eval is not supported.")]
    public Task arrow_body_derived_cls_direct_eval_contains_superproperty_2()
        => ExecutionTestFromFile("arrow-body-derived-cls-direct-eval-contains-superproperty-2");

    [Fact(DisplayName = "arrow-body-derived-cls-direct-eval-err-contains-supercall-1.js", Skip = "eval is not supported.")]
    public Task arrow_body_derived_cls_direct_eval_err_contains_supercall_1()
        => ExecutionTestFromFile("arrow-body-derived-cls-direct-eval-err-contains-supercall-1");

    [Fact(DisplayName = "arrow-body-derived-cls-direct-eval-err-contains-supercall-2.js", Skip = "eval is not supported.")]
    public Task arrow_body_derived_cls_direct_eval_err_contains_supercall_2()
        => ExecutionTestFromFile("arrow-body-derived-cls-direct-eval-err-contains-supercall-2");

    [Fact(DisplayName = "arrow-body-derived-cls-direct-eval-err-contains-supercall.js", Skip = "eval is not supported.")]
    public Task arrow_body_derived_cls_direct_eval_err_contains_supercall()
        => ExecutionTestFromFile("arrow-body-derived-cls-direct-eval-err-contains-supercall");

    [Fact(DisplayName = "arrow-body-direct-eval-err-contains-arguments.js", Skip = "eval is not supported.")]
    public Task arrow_body_direct_eval_err_contains_arguments()
        => ExecutionTestFromFile("arrow-body-direct-eval-err-contains-arguments");

    [Fact(DisplayName = "arrow-body-direct-eval-err-contains-newtarget.js", Skip = "eval is not supported.")]
    public Task arrow_body_direct_eval_err_contains_newtarget()
        => ExecutionTestFromFile("arrow-body-direct-eval-err-contains-newtarget");

    [Fact(DisplayName = "arrow-body-private-derived-cls-direct-eval-contains-superproperty-1.js", Skip = "eval is not supported.")]
    public Task arrow_body_private_derived_cls_direct_eval_contains_superproperty_1()
        => ExecutionTestFromFile("arrow-body-private-derived-cls-direct-eval-contains-superproperty-1");

    [Fact(DisplayName = "arrow-body-private-derived-cls-direct-eval-contains-superproperty-2.js", Skip = "eval is not supported.")]
    public Task arrow_body_private_derived_cls_direct_eval_contains_superproperty_2()
        => ExecutionTestFromFile("arrow-body-private-derived-cls-direct-eval-contains-superproperty-2");

    [Fact(DisplayName = "arrow-body-private-derived-cls-direct-eval-err-contains-supercall-1.js", Skip = "eval is not supported.")]
    public Task arrow_body_private_derived_cls_direct_eval_err_contains_supercall_1()
        => ExecutionTestFromFile("arrow-body-private-derived-cls-direct-eval-err-contains-supercall-1");

    [Fact(DisplayName = "arrow-body-private-derived-cls-direct-eval-err-contains-supercall-2.js", Skip = "eval is not supported.")]
    public Task arrow_body_private_derived_cls_direct_eval_err_contains_supercall_2()
        => ExecutionTestFromFile("arrow-body-private-derived-cls-direct-eval-err-contains-supercall-2");

    [Fact(DisplayName = "arrow-body-private-derived-cls-direct-eval-err-contains-supercall.js", Skip = "eval is not supported.")]
    public Task arrow_body_private_derived_cls_direct_eval_err_contains_supercall()
        => ExecutionTestFromFile("arrow-body-private-derived-cls-direct-eval-err-contains-supercall");

    [Fact(DisplayName = "arrow-body-private-direct-eval-err-contains-arguments.js", Skip = "eval is not supported.")]
    public Task arrow_body_private_direct_eval_err_contains_arguments()
        => ExecutionTestFromFile("arrow-body-private-direct-eval-err-contains-arguments");

    [Fact(DisplayName = "arrow-body-private-direct-eval-err-contains-newtarget.js", Skip = "eval is not supported.")]
    public Task arrow_body_private_direct_eval_err_contains_newtarget()
        => ExecutionTestFromFile("arrow-body-private-direct-eval-err-contains-newtarget");

    [Fact(DisplayName = "class-name-static-initializer-default-export.js")]
    public Task class_name_static_initializer_default_export()
        => ExecutionTestFromFile("class-name-static-initializer-default-export");

    [Fact(DisplayName = "derived-cls-direct-eval-contains-superproperty-1.js", Skip = "eval is not supported.")]
    public Task derived_cls_direct_eval_contains_superproperty_1()
        => ExecutionTestFromFile("derived-cls-direct-eval-contains-superproperty-1");

    [Fact(DisplayName = "derived-cls-direct-eval-contains-superproperty-2.js", Skip = "eval is not supported.")]
    public Task derived_cls_direct_eval_contains_superproperty_2()
        => ExecutionTestFromFile("derived-cls-direct-eval-contains-superproperty-2");

    [Fact(DisplayName = "derived-cls-direct-eval-err-contains-supercall-1.js", Skip = "eval is not supported.")]
    public Task derived_cls_direct_eval_err_contains_supercall_1()
        => ExecutionTestFromFile("derived-cls-direct-eval-err-contains-supercall-1");

    [Fact(DisplayName = "derived-cls-direct-eval-err-contains-supercall-2.js", Skip = "eval is not supported.")]
    public Task derived_cls_direct_eval_err_contains_supercall_2()
        => ExecutionTestFromFile("derived-cls-direct-eval-err-contains-supercall-2");

    [Fact(DisplayName = "derived-cls-direct-eval-err-contains-supercall.js", Skip = "eval is not supported.")]
    public Task derived_cls_direct_eval_err_contains_supercall()
        => ExecutionTestFromFile("derived-cls-direct-eval-err-contains-supercall");

    [Fact(DisplayName = "direct-eval-err-contains-arguments.js", Skip = "eval is not supported.")]
    public Task direct_eval_err_contains_arguments()
        => ExecutionTestFromFile("direct-eval-err-contains-arguments");

    [Fact(DisplayName = "direct-eval-err-contains-newtarget.js", Skip = "eval is not supported.")]
    public Task direct_eval_err_contains_newtarget()
        => ExecutionTestFromFile("direct-eval-err-contains-newtarget");

    [Fact(DisplayName = "field-definition-accessor-no-line-terminator.js")]
    public Task field_definition_accessor_no_line_terminator()
        => ExecutionTestFromFile("field-definition-accessor-no-line-terminator");

    [Fact(DisplayName = "grammar-private-field-optional-chaining.js")]
    public Task grammar_private_field_optional_chaining()
        => ExecutionTestFromFile("grammar-private-field-optional-chaining");

    [Fact(DisplayName = "init-value-defined-after-class.js")]
    public Task init_value_defined_after_class()
        => ExecutionTestFromFile("init-value-defined-after-class");

    [Fact(DisplayName = "init-value-incremental.js")]
    public Task init_value_incremental()
        => ExecutionTestFromFile("init-value-incremental");

    [Fact(DisplayName = "intercalated-static-non-static-computed-fields.js")]
    public Task intercalated_static_non_static_computed_fields()
        => ExecutionTestFromFile("intercalated-static-non-static-computed-fields");

    [Fact(DisplayName = "nested-derived-cls-direct-eval-contains-superproperty-1.js", Skip = "eval is not supported.")]
    public Task nested_derived_cls_direct_eval_contains_superproperty_1()
        => ExecutionTestFromFile("nested-derived-cls-direct-eval-contains-superproperty-1");

    [Fact(DisplayName = "nested-derived-cls-direct-eval-contains-superproperty-2.js", Skip = "eval is not supported.")]
    public Task nested_derived_cls_direct_eval_contains_superproperty_2()
        => ExecutionTestFromFile("nested-derived-cls-direct-eval-contains-superproperty-2");

    [Fact(DisplayName = "nested-derived-cls-direct-eval-err-contains-supercall-1.js", Skip = "eval is not supported.")]
    public Task nested_derived_cls_direct_eval_err_contains_supercall_1()
        => ExecutionTestFromFile("nested-derived-cls-direct-eval-err-contains-supercall-1");

    [Fact(DisplayName = "nested-derived-cls-direct-eval-err-contains-supercall-2.js", Skip = "eval is not supported.")]
    public Task nested_derived_cls_direct_eval_err_contains_supercall_2()
        => ExecutionTestFromFile("nested-derived-cls-direct-eval-err-contains-supercall-2");

    [Fact(DisplayName = "nested-derived-cls-direct-eval-err-contains-supercall.js", Skip = "eval is not supported.")]
    public Task nested_derived_cls_direct_eval_err_contains_supercall()
        => ExecutionTestFromFile("nested-derived-cls-direct-eval-err-contains-supercall");

    [Fact(DisplayName = "nested-direct-eval-err-contains-arguments.js", Skip = "eval is not supported.")]
    public Task nested_direct_eval_err_contains_arguments()
        => ExecutionTestFromFile("nested-direct-eval-err-contains-arguments");

    [Fact(DisplayName = "nested-direct-eval-err-contains-newtarget.js", Skip = "eval is not supported.")]
    public Task nested_direct_eval_err_contains_newtarget()
        => ExecutionTestFromFile("nested-direct-eval-err-contains-newtarget");

    [Fact(DisplayName = "nested-private-derived-cls-direct-eval-contains-superproperty-1.js", Skip = "eval is not supported.")]
    public Task nested_private_derived_cls_direct_eval_contains_superproperty_1()
        => ExecutionTestFromFile("nested-private-derived-cls-direct-eval-contains-superproperty-1");

    [Fact(DisplayName = "nested-private-derived-cls-direct-eval-contains-superproperty-2.js", Skip = "eval is not supported.")]
    public Task nested_private_derived_cls_direct_eval_contains_superproperty_2()
        => ExecutionTestFromFile("nested-private-derived-cls-direct-eval-contains-superproperty-2");

    [Fact(DisplayName = "nested-private-derived-cls-direct-eval-err-contains-supercall-1.js", Skip = "eval is not supported.")]
    public Task nested_private_derived_cls_direct_eval_err_contains_supercall_1()
        => ExecutionTestFromFile("nested-private-derived-cls-direct-eval-err-contains-supercall-1");

    [Fact(DisplayName = "nested-private-derived-cls-direct-eval-err-contains-supercall-2.js", Skip = "eval is not supported.")]
    public Task nested_private_derived_cls_direct_eval_err_contains_supercall_2()
        => ExecutionTestFromFile("nested-private-derived-cls-direct-eval-err-contains-supercall-2");

    [Fact(DisplayName = "nested-private-derived-cls-direct-eval-err-contains-supercall.js", Skip = "eval is not supported.")]
    public Task nested_private_derived_cls_direct_eval_err_contains_supercall()
        => ExecutionTestFromFile("nested-private-derived-cls-direct-eval-err-contains-supercall");

    [Fact(DisplayName = "nested-private-direct-eval-err-contains-arguments.js", Skip = "eval is not supported.")]
    public Task nested_private_direct_eval_err_contains_arguments()
        => ExecutionTestFromFile("nested-private-direct-eval-err-contains-arguments");

    [Fact(DisplayName = "nested-private-direct-eval-err-contains-newtarget.js", Skip = "eval is not supported.")]
    public Task nested_private_direct_eval_err_contains_newtarget()
        => ExecutionTestFromFile("nested-private-direct-eval-err-contains-newtarget");

    [Fact(DisplayName = "private-derived-cls-direct-eval-contains-superproperty-1.js", Skip = "eval is not supported.")]
    public Task private_derived_cls_direct_eval_contains_superproperty_1()
        => ExecutionTestFromFile("private-derived-cls-direct-eval-contains-superproperty-1");

    [Fact(DisplayName = "private-derived-cls-direct-eval-contains-superproperty-2.js", Skip = "eval is not supported.")]
    public Task private_derived_cls_direct_eval_contains_superproperty_2()
        => ExecutionTestFromFile("private-derived-cls-direct-eval-contains-superproperty-2");

    [Fact(DisplayName = "private-derived-cls-direct-eval-err-contains-supercall-1.js", Skip = "eval is not supported.")]
    public Task private_derived_cls_direct_eval_err_contains_supercall_1()
        => ExecutionTestFromFile("private-derived-cls-direct-eval-err-contains-supercall-1");

    [Fact(DisplayName = "private-derived-cls-direct-eval-err-contains-supercall-2.js", Skip = "eval is not supported.")]
    public Task private_derived_cls_direct_eval_err_contains_supercall_2()
        => ExecutionTestFromFile("private-derived-cls-direct-eval-err-contains-supercall-2");

    [Fact(DisplayName = "private-derived-cls-direct-eval-err-contains-supercall.js", Skip = "eval is not supported.")]
    public Task private_derived_cls_direct_eval_err_contains_supercall()
        => ExecutionTestFromFile("private-derived-cls-direct-eval-err-contains-supercall");

    [Fact(DisplayName = "private-direct-eval-err-contains-arguments.js", Skip = "eval is not supported.")]
    public Task private_direct_eval_err_contains_arguments()
        => ExecutionTestFromFile("private-direct-eval-err-contains-arguments");

    [Fact(DisplayName = "private-direct-eval-err-contains-newtarget.js", Skip = "eval is not supported.")]
    public Task private_direct_eval_err_contains_newtarget()
        => ExecutionTestFromFile("private-direct-eval-err-contains-newtarget");

    [Fact(DisplayName = "private-field-access-on-inner-arrow-function.js")]
    public Task private_field_access_on_inner_arrow_function()
        => ExecutionTestFromFile("private-field-access-on-inner-arrow-function");

    [Fact(DisplayName = "private-field-access-on-inner-function.js")]
    public Task private_field_access_on_inner_function()
        => ExecutionTestFromFile("private-field-access-on-inner-function");

    [Fact(DisplayName = "private-field-after-optional-chain.js")]
    public Task private_field_after_optional_chain()
        => ExecutionTestFromFile("private-field-after-optional-chain");

    [Fact(DisplayName = "private-getter-access-on-inner-arrow-function.js")]
    public Task private_getter_access_on_inner_arrow_function()
        => ExecutionTestFromFile("private-getter-access-on-inner-arrow-function");

    [Fact(DisplayName = "private-method-access-on-inner-arrow-function.js")]
    public Task private_method_access_on_inner_arrow_function()
        => ExecutionTestFromFile("private-method-access-on-inner-arrow-function");

    [Fact(DisplayName = "private-method-referenced-from-static-method.js")]
    public Task private_method_referenced_from_static_method()
        => ExecutionTestFromFile("private-method-referenced-from-static-method");

    [Fact(DisplayName = "private-setter-access-on-inner-arrow-function.js")]
    public Task private_setter_access_on_inner_arrow_function()
        => ExecutionTestFromFile("private-setter-access-on-inner-arrow-function");

    [Fact(DisplayName = "static-field-anonymous-function-name.js")]
    public Task static_field_anonymous_function_name()
        => ExecutionTestFromFile("static-field-anonymous-function-name");

    [Fact(DisplayName = "static-field-init-this-inside-arrow-function.js")]
    public Task static_field_init_this_inside_arrow_function()
        => ExecutionTestFromFile("static-field-init-this-inside-arrow-function");

    [Fact(DisplayName = "static-field-init-with-this.js", Skip = "eval is not supported.")]
    public Task static_field_init_with_this()
        => ExecutionTestFromFile("static-field-init-with-this");

    [Fact(DisplayName = "static-field-redeclaration.js")]
    public Task static_field_redeclaration()
        => ExecutionTestFromFile("static-field-redeclaration");

    [Fact(DisplayName = "static-private-getter-access-on-inner-arrow-function.js")]
    public Task static_private_getter_access_on_inner_arrow_function()
        => ExecutionTestFromFile("static-private-getter-access-on-inner-arrow-function");

    [Fact(DisplayName = "static-private-method-access-on-inner-arrow-function.js")]
    public Task static_private_method_access_on_inner_arrow_function()
        => ExecutionTestFromFile("static-private-method-access-on-inner-arrow-function");

    [Fact(DisplayName = "static-private-method-referenced-from-instance-method.js")]
    public Task static_private_method_referenced_from_instance_method()
        => ExecutionTestFromFile("static-private-method-referenced-from-instance-method");

    [Fact(DisplayName = "static-private-setter-access-on-inner-arrow-function.js")]
    public Task static_private_setter_access_on_inner_arrow_function()
        => ExecutionTestFromFile("static-private-setter-access-on-inner-arrow-function");

    [Fact(DisplayName = "super-access-from-arrow-func-on-field.js")]
    public Task super_access_from_arrow_func_on_field()
        => ExecutionTestFromFile("super-access-from-arrow-func-on-field");
}
