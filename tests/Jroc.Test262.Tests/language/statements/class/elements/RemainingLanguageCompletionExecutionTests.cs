namespace Jroc.Test262.Tests.language.statements.test_class.elements;

public sealed class RemainingLanguageCompletionExecutionTests : DiskExecutionTestsBase
{
    public RemainingLanguageCompletionExecutionTests() : base("language.statements.test_class.elements") { }

    [Fact(DisplayName = "class-field-on-frozen-objects.js")]
    public Task class_field_on_frozen_objects()
        => ExecutionTest("class-field-on-frozen-objects");

    [Fact(DisplayName = "private-accessor-is-visible-in-computed-properties.js")]
    public Task private_accessor_is_visible_in_computed_properties()
        => ExecutionTest("private-accessor-is-visible-in-computed-properties");

    [Fact(DisplayName = "private-class-field-on-nonextensible-objects.js", Skip = "Requires the unsupported nonextensible-applies-to-private proposal; ES2025 and Node.js permit private elements on non-extensible objects.")]
    public Task private_class_field_on_nonextensible_objects()
        => ExecutionTest("private-class-field-on-nonextensible-objects");

    [Fact(DisplayName = "private-field-is-visible-in-computed-properties.js")]
    public Task private_field_is_visible_in_computed_properties()
        => ExecutionTest("private-field-is-visible-in-computed-properties");

    [Fact(DisplayName = "private-field-with-initialized-id-is-visible-in-computed-properties.js")]
    public Task private_field_with_initialized_id_is_visible_in_computed_properties()
        => ExecutionTest("private-field-with-initialized-id-is-visible-in-computed-properties");

    [Fact(DisplayName = "private-getter-brand-check-multiple-evaluations-of-class.js")]
    public Task private_getter_brand_check_multiple_evaluations_of_class()
        => ExecutionTest("private-getter-brand-check-multiple-evaluations-of-class");

    [Fact(DisplayName = "private-method-brand-check-multiple-evaluations-of-class.js")]
    public Task private_method_brand_check_multiple_evaluations_of_class()
        => ExecutionTest("private-method-brand-check-multiple-evaluations-of-class");

    [Fact(DisplayName = "private-method-comparison-multiple-evaluations-of-class.js")]
    public Task private_method_comparison_multiple_evaluations_of_class()
        => ExecutionTest("private-method-comparison-multiple-evaluations-of-class");

    [Fact(DisplayName = "private-method-double-initialisation-get-and-set.js")]
    public Task private_method_double_initialisation_get_and_set()
        => ExecutionTest("private-method-double-initialisation-get-and-set");

    [Fact(DisplayName = "private-method-double-initialisation-get.js")]
    public Task private_method_double_initialisation_get()
        => ExecutionTest("private-method-double-initialisation-get");

    [Fact(DisplayName = "private-method-double-initialisation-set.js")]
    public Task private_method_double_initialisation_set()
        => ExecutionTest("private-method-double-initialisation-set");

    [Fact(DisplayName = "private-method-double-initialisation.js")]
    public Task private_method_double_initialisation()
        => ExecutionTest("private-method-double-initialisation");

    [Fact(DisplayName = "private-method-is-visible-in-computed-properties.js")]
    public Task private_method_is_visible_in_computed_properties()
        => ExecutionTest("private-method-is-visible-in-computed-properties");

    [Fact(DisplayName = "private-setter-brand-check-multiple-evaluations-of-class.js")]
    public Task private_setter_brand_check_multiple_evaluations_of_class()
        => ExecutionTest("private-setter-brand-check-multiple-evaluations-of-class");

    [Fact(DisplayName = "privatefieldadd-typeerror.js")]
    public Task privatefieldadd_typeerror()
        => ExecutionTest("privatefieldadd-typeerror");

    [Fact(DisplayName = "privatefieldget-typeerror-1.js")]
    public Task privatefieldget_typeerror_1()
        => ExecutionTest("privatefieldget-typeerror-1");

    [Fact(DisplayName = "privatefieldset-evaluation-order-1.js")]
    public Task privatefieldset_evaluation_order_1()
        => ExecutionTest("privatefieldset-evaluation-order-1");

    [Fact(DisplayName = "privatefieldset-typeerror-1.js")]
    public Task privatefieldset_typeerror_1()
        => ExecutionTest("privatefieldset-typeerror-1");

    [Fact(DisplayName = "privategetter-on-proxy.js")]
    public Task privategetter_on_proxy()
        => ExecutionTest("privategetter-on-proxy");

    [Fact(DisplayName = "privatemethods-on-proxy.js")]
    public Task privatemethods_on_proxy()
        => ExecutionTest("privatemethods-on-proxy");

    [Fact(DisplayName = "prod-private-getter-before-super-return-in-constructor.js")]
    public Task prod_private_getter_before_super_return_in_constructor()
        => ExecutionTest("prod-private-getter-before-super-return-in-constructor");

    [Fact(DisplayName = "prod-private-getter-before-super-return-in-field-initializer.js")]
    public Task prod_private_getter_before_super_return_in_field_initializer()
        => ExecutionTest("prod-private-getter-before-super-return-in-field-initializer");

    [Fact(DisplayName = "prod-private-method-before-super-return-in-constructor.js")]
    public Task prod_private_method_before_super_return_in_constructor()
        => ExecutionTest("prod-private-method-before-super-return-in-constructor");

    [Fact(DisplayName = "prod-private-method-before-super-return-in-field-initializer.js")]
    public Task prod_private_method_before_super_return_in_field_initializer()
        => ExecutionTest("prod-private-method-before-super-return-in-field-initializer");

    [Fact(DisplayName = "prod-private-setter-before-super-return-in-constructor.js")]
    public Task prod_private_setter_before_super_return_in_constructor()
        => ExecutionTest("prod-private-setter-before-super-return-in-constructor");

    [Fact(DisplayName = "prod-private-setter-before-super-return-in-field-initializer.js")]
    public Task prod_private_setter_before_super_return_in_field_initializer()
        => ExecutionTest("prod-private-setter-before-super-return-in-field-initializer");
}
