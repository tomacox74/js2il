namespace Jroc.Test262.Tests.built_ins.Object.seal;

public partial class ExecutionTests
{
    [Fact(DisplayName = "configurable-attribute-all-own-properties-set-from-true-to-false-property-are-unaltered.js")]
    public Task configurable_attribute_all_own_properties_set_from_true_to_false_property_are_unaltered() => ExecutionTestFromFile("configurable-attribute-all-own-properties-set-from-true-to-false-property-are-unaltered");

    [Fact(DisplayName = "configurable-attribute-own-accessor-property-set-from-true-to-false-property-are-unaltered.js")]
    public Task configurable_attribute_own_accessor_property_set_from_true_to_false_property_are_unaltered() => ExecutionTestFromFile("configurable-attribute-own-accessor-property-set-from-true-to-false-property-are-unaltered");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor.js")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "object-seal-all-own-properties-of-o-are-already-non-configurable.js")]
    public Task object_seal_all_own_properties_of_o_are_already_non_configurable() => ExecutionTestFromFile("object-seal-all-own-properties-of-o-are-already-non-configurable");

    [Fact(DisplayName = "object-seal-configurable-attribute-of-own-data-property-of-o-is-set-from-true-to-false-and-other-attributes-of-the-property-are-unaltered.js")]
    public Task object_seal_configurable_attribute_of_own_data_property_of_o_is_set_from_true_to_false_and_other_attributes_of_the_property_are_unaltered() => ExecutionTestFromFile("object-seal-configurable-attribute-of-own-data-property-of-o-is-set-from-true-to-false-and-other-attributes-of-the-property-are-unaltered");

    [Fact(DisplayName = "object-seal-extensible-of-o-is-set-as-false-even-if-o-has-no-own-property.js")]
    public Task object_seal_extensible_of_o_is_set_as_false_even_if_o_has_no_own_property() => ExecutionTestFromFile("object-seal-extensible-of-o-is-set-as-false-even-if-o-has-no-own-property");

    [Fact(DisplayName = "object-seal-inherited-accessor-properties-are-ignored.js")]
    public Task object_seal_inherited_accessor_properties_are_ignored() => ExecutionTestFromFile("object-seal-inherited-accessor-properties-are-ignored");

    [Fact(DisplayName = "object-seal-inherited-data-properties-are-ignored.js")]
    public Task object_seal_inherited_data_properties_are_ignored() => ExecutionTestFromFile("object-seal-inherited-data-properties-are-ignored");

    [Fact(DisplayName = "object-seal-is-a-function.js")]
    public Task object_seal_is_a_function() => ExecutionTestFromFile("object-seal-is-a-function");

    [Fact(DisplayName = "object-seal-non-enumerable-own-property-of-o-is-sealed.js")]
    public Task object_seal_non_enumerable_own_property_of_o_is_sealed() => ExecutionTestFromFile("object-seal-non-enumerable-own-property-of-o-is-sealed");

    [Fact(DisplayName = "object-seal-o-is-a-boolean-object.js")]
    public Task object_seal_o_is_a_boolean_object() => ExecutionTestFromFile("object-seal-o-is-a-boolean-object");

    [Fact(DisplayName = "object-seal-o-is-a-date-object.js")]
    public Task object_seal_o_is_a_date_object() => ExecutionTestFromFile("object-seal-o-is-a-date-object");

    [Fact(DisplayName = "object-seal-o-is-a-function-object.js")]
    public Task object_seal_o_is_a_function_object() => ExecutionTestFromFile("object-seal-o-is-a-function-object");

    [Fact(DisplayName = "object-seal-o-is-a-number-object.js")]
    public Task object_seal_o_is_a_number_object() => ExecutionTestFromFile("object-seal-o-is-a-number-object");

    [Fact(DisplayName = "object-seal-o-is-a-reg-exp-object.js")]
    public Task object_seal_o_is_a_reg_exp_object() => ExecutionTestFromFile("object-seal-o-is-a-reg-exp-object");

    [Fact(DisplayName = "object-seal-o-is-a-string-object.js")]
    public Task object_seal_o_is_a_string_object() => ExecutionTestFromFile("object-seal-o-is-a-string-object");

    [Fact(DisplayName = "object-seal-o-is-an-arguments-object.js")]
    public Task object_seal_o_is_an_arguments_object() => ExecutionTestFromFile("object-seal-o-is-an-arguments-object");

    [Fact(DisplayName = "object-seal-o-is-an-array-object.js")]
    public Task object_seal_o_is_an_array_object() => ExecutionTestFromFile("object-seal-o-is-an-array-object");

    [Fact(DisplayName = "object-seal-o-is-frozen-already.js")]
    public Task object_seal_o_is_frozen_already() => ExecutionTestFromFile("object-seal-o-is-frozen-already");

    [Fact(DisplayName = "object-seal-o-is-sealed-already.js")]
    public Task object_seal_o_is_sealed_already() => ExecutionTestFromFile("object-seal-o-is-sealed-already");

    [Fact(DisplayName = "object-seal-p-is-own-accessor-property-that-overrides-an-inherited-accessor-property.js")]
    public Task object_seal_p_is_own_accessor_property_that_overrides_an_inherited_accessor_property() => ExecutionTestFromFile("object-seal-p-is-own-accessor-property-that-overrides-an-inherited-accessor-property");

    [Fact(DisplayName = "object-seal-p-is-own-accessor-property-that-overrides-an-inherited-data-property.js")]
    public Task object_seal_p_is_own_accessor_property_that_overrides_an_inherited_data_property() => ExecutionTestFromFile("object-seal-p-is-own-accessor-property-that-overrides-an-inherited-data-property");

    [Fact(DisplayName = "object-seal-p-is-own-accessor-property.js")]
    public Task object_seal_p_is_own_accessor_property() => ExecutionTestFromFile("object-seal-p-is-own-accessor-property");

    [Fact(DisplayName = "object-seal-p-is-own-data-property-that-overrides-an-inherited-accessor-property.js")]
    public Task object_seal_p_is_own_data_property_that_overrides_an_inherited_accessor_property() => ExecutionTestFromFile("object-seal-p-is-own-data-property-that-overrides-an-inherited-accessor-property");

    [Fact(DisplayName = "object-seal-p-is-own-data-property-that-overrides-an-inherited-data-property.js")]
    public Task object_seal_p_is_own_data_property_that_overrides_an_inherited_data_property() => ExecutionTestFromFile("object-seal-p-is-own-data-property-that-overrides-an-inherited-data-property");

    [Fact(DisplayName = "object-seal-p-is-own-data-property.js")]
    public Task object_seal_p_is_own_data_property() => ExecutionTestFromFile("object-seal-p-is-own-data-property");

    [Fact(DisplayName = "object-seal-p-is-own-property-of-a-boolean-object-that-uses-object-s-get-own-property.js")]
    public Task object_seal_p_is_own_property_of_a_boolean_object_that_uses_object_s_get_own_property() => ExecutionTestFromFile("object-seal-p-is-own-property-of-a-boolean-object-that-uses-object-s-get-own-property");

    [Fact(DisplayName = "object-seal-p-is-own-property-of-a-date-object-that-uses-object-s-get-own-property.js")]
    public Task object_seal_p_is_own_property_of_a_date_object_that_uses_object_s_get_own_property() => ExecutionTestFromFile("object-seal-p-is-own-property-of-a-date-object-that-uses-object-s-get-own-property");

    [Fact(DisplayName = "object-seal-p-is-own-property-of-a-function-object-that-uses-object-s-get-own-property.js")]
    public Task object_seal_p_is_own_property_of_a_function_object_that_uses_object_s_get_own_property() => ExecutionTestFromFile("object-seal-p-is-own-property-of-a-function-object-that-uses-object-s-get-own-property");

    [Fact(DisplayName = "object-seal-p-is-own-property-of-a-number-object-that-uses-object-s-get-own-property.js")]
    public Task object_seal_p_is_own_property_of_a_number_object_that_uses_object_s_get_own_property() => ExecutionTestFromFile("object-seal-p-is-own-property-of-a-number-object-that-uses-object-s-get-own-property");

    [Fact(DisplayName = "object-seal-p-is-own-property-of-a-reg-exp-object-that-uses-object-s-get-own-property.js")]
    public Task object_seal_p_is_own_property_of_a_reg_exp_object_that_uses_object_s_get_own_property() => ExecutionTestFromFile("object-seal-p-is-own-property-of-a-reg-exp-object-that-uses-object-s-get-own-property");

    [Fact(DisplayName = "object-seal-p-is-own-property-of-a-string-object-which-implements-its-own-get-own-property.js")]
    public Task object_seal_p_is_own_property_of_a_string_object_which_implements_its_own_get_own_property() => ExecutionTestFromFile("object-seal-p-is-own-property-of-a-string-object-which-implements-its-own-get-own-property");

    [Fact(DisplayName = "object-seal-p-is-own-property-of-an-arguments-object-which-implements-its-own-get-own-property.js")]
    public Task object_seal_p_is_own_property_of_an_arguments_object_which_implements_its_own_get_own_property() => ExecutionTestFromFile("object-seal-p-is-own-property-of-an-arguments-object-which-implements-its-own-get-own-property");

    [Fact(DisplayName = "object-seal-p-is-own-property-of-an-array-object-that-uses-object-s-get-own-property.js")]
    public Task object_seal_p_is_own_property_of_an_array_object_that_uses_object_s_get_own_property() => ExecutionTestFromFile("object-seal-p-is-own-property-of-an-array-object-that-uses-object-s-get-own-property");

    [Fact(DisplayName = "object-seal-p-is-own-property-of-an-error-object-that-uses-object-s-get-own-property.js")]
    public Task object_seal_p_is_own_property_of_an_error_object_that_uses_object_s_get_own_property() => ExecutionTestFromFile("object-seal-p-is-own-property-of-an-error-object-that-uses-object-s-get-own-property");

    [Fact(DisplayName = "object-seal-returned-object-is-not-extensible.js")]
    public Task object_seal_returned_object_is_not_extensible() => ExecutionTestFromFile("object-seal-returned-object-is-not-extensible");

    [Fact(DisplayName = "object-seal-the-extension-of-o-is-prevented-already.js")]
    public Task object_seal_the_extension_of_o_is_prevented_already() => ExecutionTestFromFile("object-seal-the-extension-of-o-is-prevented-already");

    [Fact(DisplayName = "seal-aggregateerror.js")]
    public Task seal_aggregateerror() => ExecutionTestFromFile("seal-aggregateerror");

    [Fact(DisplayName = "seal-arraybuffer.js")]
    public Task seal_arraybuffer() => ExecutionTestFromFile("seal-arraybuffer");

    [Fact(DisplayName = "seal-asyncarrowfunction.js")]
    public Task seal_asyncarrowfunction() => ExecutionTestFromFile("seal-asyncarrowfunction");

    [Fact(DisplayName = "seal-asyncfunction.js")]
    public Task seal_asyncfunction() => ExecutionTestFromFile("seal-asyncfunction");

    [Fact(DisplayName = "seal-asyncgeneratorfunction.js")]
    public Task seal_asyncgeneratorfunction() => ExecutionTestFromFile("seal-asyncgeneratorfunction");

    [Fact(DisplayName = "seal-bigint64array.js")]
    public Task seal_bigint64array() => ExecutionTestFromFile("seal-bigint64array");

    [Fact(DisplayName = "seal-biguint64array.js")]
    public Task seal_biguint64array() => ExecutionTestFromFile("seal-biguint64array");

    [Fact(DisplayName = "seal-boolean-literal.js")]
    public Task seal_boolean_literal() => ExecutionTestFromFile("seal-boolean-literal");

    [Fact(DisplayName = "seal-boolean.js")]
    public Task seal_boolean() => ExecutionTestFromFile("seal-boolean");

    [Fact(DisplayName = "seal-dataview.js")]
    public Task seal_dataview() => ExecutionTestFromFile("seal-dataview");

    [Fact(DisplayName = "seal-date.js")]
    public Task seal_date() => ExecutionTestFromFile("seal-date");

    [Fact(DisplayName = "seal-error.js")]
    public Task seal_error() => ExecutionTestFromFile("seal-error");

    [Fact(DisplayName = "seal-evalerror.js")]
    public Task seal_evalerror() => ExecutionTestFromFile("seal-evalerror");

    [Fact(DisplayName = "seal-finalizationregistry.js")]
    public Task seal_finalizationregistry() => ExecutionTestFromFile("seal-finalizationregistry");

    [Fact(DisplayName = "seal-float32array.js")]
    public Task seal_float32array() => ExecutionTestFromFile("seal-float32array");

    [Fact(DisplayName = "seal-float64array.js")]
    public Task seal_float64array() => ExecutionTestFromFile("seal-float64array");

    [Fact(DisplayName = "seal-function.js")]
    public Task seal_function() => ExecutionTestFromFile("seal-function");

    [Fact(DisplayName = "seal-generatorfunction.js")]
    public Task seal_generatorfunction() => ExecutionTestFromFile("seal-generatorfunction");

    [Fact(DisplayName = "seal-infinity.js")]
    public Task seal_infinity() => ExecutionTestFromFile("seal-infinity");

    [Fact(DisplayName = "seal-int16array.js")]
    public Task seal_int16array() => ExecutionTestFromFile("seal-int16array");

    [Fact(DisplayName = "seal-int32array.js")]
    public Task seal_int32array() => ExecutionTestFromFile("seal-int32array");

    [Fact(DisplayName = "seal-int8array.js")]
    public Task seal_int8array() => ExecutionTestFromFile("seal-int8array");

    [Fact(DisplayName = "seal-map.js")]
    public Task seal_map() => ExecutionTestFromFile("seal-map");

    [Fact(DisplayName = "seal-nan.js")]
    public Task seal_nan() => ExecutionTestFromFile("seal-nan");

    [Fact(DisplayName = "seal-null.js")]
    public Task seal_null() => ExecutionTestFromFile("seal-null");

    [Fact(DisplayName = "seal-number.js")]
    public Task seal_number() => ExecutionTestFromFile("seal-number");

    [Fact(DisplayName = "seal-object.js")]
    public Task seal_object() => ExecutionTestFromFile("seal-object");

    [Fact(DisplayName = "seal-promise.js")]
    public Task seal_promise() => ExecutionTestFromFile("seal-promise");

    [Fact(DisplayName = "seal-proxy.js")]
    public Task seal_proxy() => ExecutionTestFromFile("seal-proxy");

    [Fact(DisplayName = "seal-rangeerror.js")]
    public Task seal_rangeerror() => ExecutionTestFromFile("seal-rangeerror");

    [Fact(DisplayName = "seal-referenceerror.js")]
    public Task seal_referenceerror() => ExecutionTestFromFile("seal-referenceerror");

    [Fact(DisplayName = "seal-regexp.js")]
    public Task seal_regexp() => ExecutionTestFromFile("seal-regexp");

    [Fact(DisplayName = "seal-set.js")]
    public Task seal_set() => ExecutionTestFromFile("seal-set");

    [Fact(DisplayName = "seal-sharedarraybuffer.js")]
    public Task seal_sharedarraybuffer() => ExecutionTestFromFile("seal-sharedarraybuffer");

    [Fact(DisplayName = "seal-string.js")]
    public Task seal_string() => ExecutionTestFromFile("seal-string");

    [Fact(DisplayName = "seal-symbol.js")]
    public Task seal_symbol() => ExecutionTestFromFile("seal-symbol");

    [Fact(DisplayName = "seal-syntaxerror.js")]
    public Task seal_syntaxerror() => ExecutionTestFromFile("seal-syntaxerror");

    [Fact(DisplayName = "seal-typeerror.js")]
    public Task seal_typeerror() => ExecutionTestFromFile("seal-typeerror");

    [Fact(DisplayName = "seal-uint16array.js")]
    public Task seal_uint16array() => ExecutionTestFromFile("seal-uint16array");

    [Fact(DisplayName = "seal-uint32array.js")]
    public Task seal_uint32array() => ExecutionTestFromFile("seal-uint32array");

    [Fact(DisplayName = "seal-uint8array.js")]
    public Task seal_uint8array() => ExecutionTestFromFile("seal-uint8array");

    [Fact(DisplayName = "seal-uint8clampedarray.js")]
    public Task seal_uint8clampedarray() => ExecutionTestFromFile("seal-uint8clampedarray");

    [Fact(DisplayName = "seal-undefined.js")]
    public Task seal_undefined() => ExecutionTestFromFile("seal-undefined");

    [Fact(DisplayName = "seal-urierror.js")]
    public Task seal_urierror() => ExecutionTestFromFile("seal-urierror");

    [Fact(DisplayName = "seal-weakmap.js")]
    public Task seal_weakmap() => ExecutionTestFromFile("seal-weakmap");

    [Fact(DisplayName = "seal-weakref.js")]
    public Task seal_weakref() => ExecutionTestFromFile("seal-weakref");

    [Fact(DisplayName = "seal-weakset.js")]
    public Task seal_weakset() => ExecutionTestFromFile("seal-weakset");

    [Fact(DisplayName = "symbol-object-contains-symbol-properties-non-strict.js")]
    public Task symbol_object_contains_symbol_properties_non_strict() => ExecutionTestFromFile("symbol-object-contains-symbol-properties-non-strict");

    [Fact(DisplayName = "symbol-object-contains-symbol-properties-strict.js")]
    public Task symbol_object_contains_symbol_properties_strict() => ExecutionTestFromFile("symbol-object-contains-symbol-properties-strict");
}
