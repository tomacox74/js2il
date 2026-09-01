namespace Jroc.Test262.Tests.built_ins.Object.prototype;

public partial class ExecutionTests
{
    [Fact(DisplayName = "S15.2.3.1_A2.js")]
    public Task S15_2_3_1_A2() => ExecutionTestFromFile("S15.2.3.1_A2");

    [Fact(DisplayName = "S15.2.4_A1_T1.js")]
    public Task S15_2_4_A1_T1() => ExecutionTestFromFile("S15.2.4_A1_T1");

    [Fact(DisplayName = "S15.2.4_A1_T2.js")]
    public Task S15_2_4_A1_T2() => ExecutionTestFromFile("S15.2.4_A1_T2");

    [Fact(DisplayName = "S15.2.4_A2.js")]
    public Task S15_2_4_A2() => ExecutionTestFromFile("S15.2.4_A2");

    [Fact(DisplayName = "S15.2.4_A3.js")]
    public Task S15_2_4_A3() => ExecutionTestFromFile("S15.2.4_A3");

    [Fact(DisplayName = "S15.2.4_A4.js")]
    public Task S15_2_4_A4() => ExecutionTestFromFile("S15.2.4_A4");

    [Fact(DisplayName = "__defineGetter__/define-new.js")]
    public Task __defineGetter___define_new() => ExecutionTestFromFile("__defineGetter__/define-new");

    [Fact(DisplayName = "__defineGetter__/getter-non-callable.js")]
    public Task __defineGetter___getter_non_callable() => ExecutionTestFromFile("__defineGetter__/getter-non-callable");

    [Fact(DisplayName = "__defineGetter__/key-invalid.js")]
    public Task __defineGetter___key_invalid() => ExecutionTestFromFile("__defineGetter__/key-invalid");

    [Fact(DisplayName = "__defineGetter__/length.js")]
    public Task __defineGetter___length() => ExecutionTestFromFile("__defineGetter__/length");

    [Fact(DisplayName = "__defineGetter__/prop-desc.js")]
    public Task __defineGetter___prop_desc() => ExecutionTestFromFile("__defineGetter__/prop-desc");

    [Fact(DisplayName = "__defineGetter__/this-non-obj.js")]
    public Task __defineGetter___this_non_obj() => ExecutionTestFromFile("__defineGetter__/this-non-obj");

    [Fact(DisplayName = "__defineSetter__/define-new.js")]
    public Task __defineSetter___define_new() => ExecutionTestFromFile("__defineSetter__/define-new");

    [Fact(DisplayName = "__defineSetter__/key-invalid.js")]
    public Task __defineSetter___key_invalid() => ExecutionTestFromFile("__defineSetter__/key-invalid");

    [Fact(DisplayName = "__defineSetter__/length.js")]
    public Task __defineSetter___length() => ExecutionTestFromFile("__defineSetter__/length");

    [Fact(DisplayName = "__defineSetter__/prop-desc.js")]
    public Task __defineSetter___prop_desc() => ExecutionTestFromFile("__defineSetter__/prop-desc");

    [Fact(DisplayName = "__defineSetter__/setter-non-callable.js")]
    public Task __defineSetter___setter_non_callable() => ExecutionTestFromFile("__defineSetter__/setter-non-callable");

    [Fact(DisplayName = "__defineSetter__/this-non-obj.js")]
    public Task __defineSetter___this_non_obj() => ExecutionTestFromFile("__defineSetter__/this-non-obj");

    [Fact(DisplayName = "__lookupGetter__/key-invalid.js")]
    public Task __lookupGetter___key_invalid() => ExecutionTestFromFile("__lookupGetter__/key-invalid");

    [Fact(DisplayName = "__lookupGetter__/length.js")]
    public Task __lookupGetter___length() => ExecutionTestFromFile("__lookupGetter__/length");

    [Fact(DisplayName = "__lookupGetter__/lookup-not-found.js")]
    public Task __lookupGetter___lookup_not_found() => ExecutionTestFromFile("__lookupGetter__/lookup-not-found");

    [Fact(DisplayName = "__lookupGetter__/lookup-own-acsr-w-getter.js")]
    public Task __lookupGetter___lookup_own_acsr_w_getter() => ExecutionTestFromFile("__lookupGetter__/lookup-own-acsr-w-getter");

    [Fact(DisplayName = "__lookupGetter__/lookup-own-acsr-wo-getter.js")]
    public Task __lookupGetter___lookup_own_acsr_wo_getter() => ExecutionTestFromFile("__lookupGetter__/lookup-own-acsr-wo-getter");

    [Fact(DisplayName = "__lookupGetter__/lookup-own-get-err.js")]
    public Task __lookupGetter___lookup_own_get_err() => ExecutionTestFromFile("__lookupGetter__/lookup-own-get-err");

    [Fact(DisplayName = "__lookupGetter__/lookup-proto-acsr-w-getter.js")]
    public Task __lookupGetter___lookup_proto_acsr_w_getter() => ExecutionTestFromFile("__lookupGetter__/lookup-proto-acsr-w-getter");

    [Fact(DisplayName = "__lookupGetter__/lookup-proto-acsr-wo-getter.js")]
    public Task __lookupGetter___lookup_proto_acsr_wo_getter() => ExecutionTestFromFile("__lookupGetter__/lookup-proto-acsr-wo-getter");

    [Fact(DisplayName = "__lookupGetter__/prop-desc.js")]
    public Task __lookupGetter___prop_desc() => ExecutionTestFromFile("__lookupGetter__/prop-desc");

    [Fact(DisplayName = "__lookupGetter__/this-non-obj.js")]
    public Task __lookupGetter___this_non_obj() => ExecutionTestFromFile("__lookupGetter__/this-non-obj");

    [Fact(DisplayName = "__lookupSetter__/key-invalid.js")]
    public Task __lookupSetter___key_invalid() => ExecutionTestFromFile("__lookupSetter__/key-invalid");

    [Fact(DisplayName = "__lookupSetter__/length.js")]
    public Task __lookupSetter___length() => ExecutionTestFromFile("__lookupSetter__/length");

    [Fact(DisplayName = "__lookupSetter__/lookup-not-found.js")]
    public Task __lookupSetter___lookup_not_found() => ExecutionTestFromFile("__lookupSetter__/lookup-not-found");

    [Fact(DisplayName = "__lookupSetter__/lookup-own-acsr-w-setter.js")]
    public Task __lookupSetter___lookup_own_acsr_w_setter() => ExecutionTestFromFile("__lookupSetter__/lookup-own-acsr-w-setter");

    [Fact(DisplayName = "__lookupSetter__/lookup-own-acsr-wo-setter.js")]
    public Task __lookupSetter___lookup_own_acsr_wo_setter() => ExecutionTestFromFile("__lookupSetter__/lookup-own-acsr-wo-setter");

    [Fact(DisplayName = "__lookupSetter__/lookup-own-get-err.js")]
    public Task __lookupSetter___lookup_own_get_err() => ExecutionTestFromFile("__lookupSetter__/lookup-own-get-err");

    [Fact(DisplayName = "__lookupSetter__/lookup-proto-acsr-w-setter.js")]
    public Task __lookupSetter___lookup_proto_acsr_w_setter() => ExecutionTestFromFile("__lookupSetter__/lookup-proto-acsr-w-setter");

    [Fact(DisplayName = "__lookupSetter__/lookup-proto-acsr-wo-setter.js")]
    public Task __lookupSetter___lookup_proto_acsr_wo_setter() => ExecutionTestFromFile("__lookupSetter__/lookup-proto-acsr-wo-setter");

    [Fact(DisplayName = "__lookupSetter__/prop-desc.js")]
    public Task __lookupSetter___prop_desc() => ExecutionTestFromFile("__lookupSetter__/prop-desc");

    [Fact(DisplayName = "__lookupSetter__/this-non-obj.js")]
    public Task __lookupSetter___this_non_obj() => ExecutionTestFromFile("__lookupSetter__/this-non-obj");

    [Fact(DisplayName = "__proto__/get-abrupt.js")]
    public Task __proto___get_abrupt() => ExecutionTestFromFile("__proto__/get-abrupt");

    [Fact(DisplayName = "__proto__/set-cycle-shadowed.js")]
    public Task __proto___set_cycle_shadowed() => ExecutionTestFromFile("__proto__/set-cycle-shadowed");

    [Fact(DisplayName = "constructor/S15.2.4.1_A1_T1.js")]
    public Task constructor_S15_2_4_1_A1_T1() => ExecutionTestFromFile("constructor/S15.2.4.1_A1_T1");

    [Fact(DisplayName = "constructor/S15.2.4.1_A1_T2.js")]
    public Task constructor_S15_2_4_1_A1_T2() => ExecutionTestFromFile("constructor/S15.2.4.1_A1_T2");

    [Fact(DisplayName = "extensibility.js")]
    public Task extensibility() => ExecutionTestFromFile("extensibility");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_16.js")]
    public Task hasOwnProperty_8_12_1_1_16() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_16");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_17.js")]
    public Task hasOwnProperty_8_12_1_1_17() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_17");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_18.js")]
    public Task hasOwnProperty_8_12_1_1_18() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_18");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_19.js")]
    public Task hasOwnProperty_8_12_1_1_19() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_19");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_20.js")]
    public Task hasOwnProperty_8_12_1_1_20() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_20");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_21.js")]
    public Task hasOwnProperty_8_12_1_1_21() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_21");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_22.js")]
    public Task hasOwnProperty_8_12_1_1_22() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_22");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_23.js")]
    public Task hasOwnProperty_8_12_1_1_23() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_23");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_24.js")]
    public Task hasOwnProperty_8_12_1_1_24() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_24");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_25.js")]
    public Task hasOwnProperty_8_12_1_1_25() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_25");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_26.js")]
    public Task hasOwnProperty_8_12_1_1_26() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_26");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_27.js")]
    public Task hasOwnProperty_8_12_1_1_27() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_27");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_28.js")]
    public Task hasOwnProperty_8_12_1_1_28() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_28");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_29.js")]
    public Task hasOwnProperty_8_12_1_1_29() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_29");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_30.js")]
    public Task hasOwnProperty_8_12_1_1_30() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_30");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_31.js")]
    public Task hasOwnProperty_8_12_1_1_31() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_31");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_32.js")]
    public Task hasOwnProperty_8_12_1_1_32() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_32");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_33.js")]
    public Task hasOwnProperty_8_12_1_1_33() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_33");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_34.js")]
    public Task hasOwnProperty_8_12_1_1_34() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_34");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_35.js")]
    public Task hasOwnProperty_8_12_1_1_35() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_35");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_36.js")]
    public Task hasOwnProperty_8_12_1_1_36() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_36");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_37.js")]
    public Task hasOwnProperty_8_12_1_1_37() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_37");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_38.js")]
    public Task hasOwnProperty_8_12_1_1_38() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_38");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_39.js")]
    public Task hasOwnProperty_8_12_1_1_39() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_39");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_40.js")]
    public Task hasOwnProperty_8_12_1_1_40() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_40");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_41.js")]
    public Task hasOwnProperty_8_12_1_1_41() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_41");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_42.js")]
    public Task hasOwnProperty_8_12_1_1_42() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_42");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_43.js")]
    public Task hasOwnProperty_8_12_1_1_43() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_43");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_44.js")]
    public Task hasOwnProperty_8_12_1_1_44() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_44");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_45.js")]
    public Task hasOwnProperty_8_12_1_1_45() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_45");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_46.js")]
    public Task hasOwnProperty_8_12_1_1_46() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_46");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_47.js")]
    public Task hasOwnProperty_8_12_1_1_47() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_47");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_48.js")]
    public Task hasOwnProperty_8_12_1_1_48() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_48");

    [Fact(DisplayName = "hasOwnProperty/8.12.1-1_49.js")]
    public Task hasOwnProperty_8_12_1_1_49() => ExecutionTestFromFile("hasOwnProperty/8.12.1-1_49");

    [Fact(DisplayName = "hasOwnProperty/S15.2.4.5_A6.js")]
    public Task hasOwnProperty_S15_2_4_5_A6() => ExecutionTestFromFile("hasOwnProperty/S15.2.4.5_A6");

    [Fact(DisplayName = "hasOwnProperty/not-a-constructor.js")]
    public Task hasOwnProperty_not_a_constructor() => ExecutionTestFromFile("hasOwnProperty/not-a-constructor");

    [Fact(DisplayName = "isPrototypeOf/builtin.js")]
    public Task isPrototypeOf_builtin() => ExecutionTestFromFile("isPrototypeOf/builtin");

    [Fact(DisplayName = "isPrototypeOf/length.js")]
    public Task isPrototypeOf_length() => ExecutionTestFromFile("isPrototypeOf/length");

    [Fact(DisplayName = "isPrototypeOf/not-a-constructor.js")]
    public Task isPrototypeOf_not_a_constructor() => ExecutionTestFromFile("isPrototypeOf/not-a-constructor");

    [Fact(DisplayName = "isPrototypeOf/null-this-and-object-arg-throws.js")]
    public Task isPrototypeOf_null_this_and_object_arg_throws() => ExecutionTestFromFile("isPrototypeOf/null-this-and-object-arg-throws");

    [Fact(DisplayName = "isPrototypeOf/this-value-is-in-prototype-chain-of-arg.js")]
    public Task isPrototypeOf_this_value_is_in_prototype_chain_of_arg() => ExecutionTestFromFile("isPrototypeOf/this-value-is-in-prototype-chain-of-arg");

    [Fact(DisplayName = "isPrototypeOf/undefined-this-and-object-arg-throws.js")]
    public Task isPrototypeOf_undefined_this_and_object_arg_throws() => ExecutionTestFromFile("isPrototypeOf/undefined-this-and-object-arg-throws");

    [Fact(DisplayName = "propertyIsEnumerable/S15.2.4.7_A10.js")]
    public Task propertyIsEnumerable_S15_2_4_7_A10() => ExecutionTestFromFile("propertyIsEnumerable/S15.2.4.7_A10");

    [Fact(DisplayName = "propertyIsEnumerable/S15.2.4.7_A12.js")]
    public Task propertyIsEnumerable_S15_2_4_7_A12() => ExecutionTestFromFile("propertyIsEnumerable/S15.2.4.7_A12");

    [Fact(DisplayName = "propertyIsEnumerable/S15.2.4.7_A13.js")]
    public Task propertyIsEnumerable_S15_2_4_7_A13() => ExecutionTestFromFile("propertyIsEnumerable/S15.2.4.7_A13");

    [Fact(DisplayName = "propertyIsEnumerable/S15.2.4.7_A1_T1.js")]
    public Task propertyIsEnumerable_S15_2_4_7_A1_T1() => ExecutionTestFromFile("propertyIsEnumerable/S15.2.4.7_A1_T1");

    [Fact(DisplayName = "propertyIsEnumerable/S15.2.4.7_A2_T1.js")]
    public Task propertyIsEnumerable_S15_2_4_7_A2_T1() => ExecutionTestFromFile("propertyIsEnumerable/S15.2.4.7_A2_T1");

    [Fact(DisplayName = "propertyIsEnumerable/S15.2.4.7_A2_T2.js")]
    public Task propertyIsEnumerable_S15_2_4_7_A2_T2() => ExecutionTestFromFile("propertyIsEnumerable/S15.2.4.7_A2_T2");

    [Fact(DisplayName = "propertyIsEnumerable/S15.2.4.7_A6.js")]
    public Task propertyIsEnumerable_S15_2_4_7_A6() => ExecutionTestFromFile("propertyIsEnumerable/S15.2.4.7_A6");

    [Fact(DisplayName = "propertyIsEnumerable/S15.2.4.7_A8.js")]
    public Task propertyIsEnumerable_S15_2_4_7_A8() => ExecutionTestFromFile("propertyIsEnumerable/S15.2.4.7_A8");

    [Fact(DisplayName = "propertyIsEnumerable/S15.2.4.7_A9.js")]
    public Task propertyIsEnumerable_S15_2_4_7_A9() => ExecutionTestFromFile("propertyIsEnumerable/S15.2.4.7_A9");

    [Fact(DisplayName = "propertyIsEnumerable/not-a-constructor.js")]
    public Task propertyIsEnumerable_not_a_constructor() => ExecutionTestFromFile("propertyIsEnumerable/not-a-constructor");

    [Fact(DisplayName = "propertyIsEnumerable/symbol_own_property.js")]
    public Task propertyIsEnumerable_symbol_own_property() => ExecutionTestFromFile("propertyIsEnumerable/symbol_own_property");

    [Fact(DisplayName = "proto.js")]
    public Task proto() => ExecutionTestFromFile("proto");

    [Fact(DisplayName = "setPrototypeOf-with-different-values.js")]
    public Task setPrototypeOf_with_different_values() => ExecutionTestFromFile("setPrototypeOf-with-different-values");

    [Fact(DisplayName = "setPrototypeOf-with-non-circular-values-__proto__.js")]
    public Task setPrototypeOf_with_non_circular_values___proto__() => ExecutionTestFromFile("setPrototypeOf-with-non-circular-values-__proto__");

    [Fact(DisplayName = "setPrototypeOf-with-non-circular-values.js")]
    public Task setPrototypeOf_with_non_circular_values() => ExecutionTestFromFile("setPrototypeOf-with-non-circular-values");

    [Fact(DisplayName = "setPrototypeOf-with-same-value.js")]
    public Task setPrototypeOf_with_same_value() => ExecutionTestFromFile("setPrototypeOf-with-same-value");

    [Fact(DisplayName = "toLocaleString/S15.2.4.3_A1.js")]
    public Task toLocaleString_S15_2_4_3_A1() => ExecutionTestFromFile("toLocaleString/S15.2.4.3_A1");

    [Fact(DisplayName = "toLocaleString/S15.2.4.3_A10.js")]
    public Task toLocaleString_S15_2_4_3_A10() => ExecutionTestFromFile("toLocaleString/S15.2.4.3_A10");

    [Fact(DisplayName = "toLocaleString/S15.2.4.3_A11.js")]
    public Task toLocaleString_S15_2_4_3_A11() => ExecutionTestFromFile("toLocaleString/S15.2.4.3_A11");

    [Fact(DisplayName = "toLocaleString/S15.2.4.3_A12.js")]
    public Task toLocaleString_S15_2_4_3_A12() => ExecutionTestFromFile("toLocaleString/S15.2.4.3_A12");

    [Fact(DisplayName = "toLocaleString/S15.2.4.3_A13.js")]
    public Task toLocaleString_S15_2_4_3_A13() => ExecutionTestFromFile("toLocaleString/S15.2.4.3_A13");

    [Fact(DisplayName = "toLocaleString/S15.2.4.3_A6.js")]
    public Task toLocaleString_S15_2_4_3_A6() => ExecutionTestFromFile("toLocaleString/S15.2.4.3_A6");

    [Fact(DisplayName = "toLocaleString/S15.2.4.3_A8.js")]
    public Task toLocaleString_S15_2_4_3_A8() => ExecutionTestFromFile("toLocaleString/S15.2.4.3_A8");

    [Fact(DisplayName = "toLocaleString/S15.2.4.3_A9.js")]
    public Task toLocaleString_S15_2_4_3_A9() => ExecutionTestFromFile("toLocaleString/S15.2.4.3_A9");

    [Fact(DisplayName = "toLocaleString/not-a-constructor.js")]
    public Task toLocaleString_not_a_constructor() => ExecutionTestFromFile("toLocaleString/not-a-constructor");

    [Fact(DisplayName = "toString/Object.prototype.toString.call-arguments.js")]
    public Task toString_Object_prototype_toString_call_arguments() => ExecutionTestFromFile("toString/Object.prototype.toString.call-arguments");

    [Fact(DisplayName = "toString/Object.prototype.toString.call-array.js")]
    public Task toString_Object_prototype_toString_call_array() => ExecutionTestFromFile("toString/Object.prototype.toString.call-array");

    [Fact(DisplayName = "toString/Object.prototype.toString.call-boolean.js")]
    public Task toString_Object_prototype_toString_call_boolean() => ExecutionTestFromFile("toString/Object.prototype.toString.call-boolean");

    [Fact(DisplayName = "toString/Object.prototype.toString.call-date.js")]
    public Task toString_Object_prototype_toString_call_date() => ExecutionTestFromFile("toString/Object.prototype.toString.call-date");

    [Fact(DisplayName = "toString/Object.prototype.toString.call-error.js")]
    public Task toString_Object_prototype_toString_call_error() => ExecutionTestFromFile("toString/Object.prototype.toString.call-error");

    [Fact(DisplayName = "toString/Object.prototype.toString.call-function.js")]
    public Task toString_Object_prototype_toString_call_function() => ExecutionTestFromFile("toString/Object.prototype.toString.call-function");

    [Fact(DisplayName = "toString/Object.prototype.toString.call-null.js")]
    public Task toString_Object_prototype_toString_call_null() => ExecutionTestFromFile("toString/Object.prototype.toString.call-null");

    [Fact(DisplayName = "toString/Object.prototype.toString.call-number.js")]
    public Task toString_Object_prototype_toString_call_number() => ExecutionTestFromFile("toString/Object.prototype.toString.call-number");

    [Fact(DisplayName = "toString/Object.prototype.toString.call-object.js")]
    public Task toString_Object_prototype_toString_call_object() => ExecutionTestFromFile("toString/Object.prototype.toString.call-object");

    [Fact(DisplayName = "toString/Object.prototype.toString.call-regexp.js")]
    public Task toString_Object_prototype_toString_call_regexp() => ExecutionTestFromFile("toString/Object.prototype.toString.call-regexp");

    [Fact(DisplayName = "toString/Object.prototype.toString.call-string.js")]
    public Task toString_Object_prototype_toString_call_string() => ExecutionTestFromFile("toString/Object.prototype.toString.call-string");

    [Fact(DisplayName = "toString/Object.prototype.toString.call-undefined.js")]
    public Task toString_Object_prototype_toString_call_undefined() => ExecutionTestFromFile("toString/Object.prototype.toString.call-undefined");

    [Fact(DisplayName = "toString/direct-invocation.js")]
    public Task toString_direct_invocation() => ExecutionTestFromFile("toString/direct-invocation");

    [Fact(DisplayName = "toString/get-symbol-tag-err.js")]
    public Task toString_get_symbol_tag_err() => ExecutionTestFromFile("toString/get-symbol-tag-err");

    [Fact(DisplayName = "toString/length.js")]
    public Task toString_length() => ExecutionTestFromFile("toString/length");

    [Fact(DisplayName = "toString/no-prototype-property.js")]
    public Task toString_no_prototype_property() => ExecutionTestFromFile("toString/no-prototype-property");

    [Fact(DisplayName = "toString/not-a-constructor.js")]
    public Task toString_not_a_constructor() => ExecutionTestFromFile("toString/not-a-constructor");

    [Fact(DisplayName = "toString/prop-desc.js")]
    public Task toString_prop_desc() => ExecutionTestFromFile("toString/prop-desc");

    [Fact(DisplayName = "toString/proxy-function-async.js")]
    public Task toString_proxy_function_async() => ExecutionTestFromFile("toString/proxy-function-async");

    [Fact(DisplayName = "toString/proxy-revoked.js")]
    public Task toString_proxy_revoked() => ExecutionTestFromFile("toString/proxy-revoked");

    [Fact(DisplayName = "toString/symbol-tag-array-builtin.js")]
    public Task toString_symbol_tag_array_builtin() => ExecutionTestFromFile("toString/symbol-tag-array-builtin");

    [Fact(DisplayName = "toString/symbol-tag-non-str-bigint.js")]
    public Task toString_symbol_tag_non_str_bigint() => ExecutionTestFromFile("toString/symbol-tag-non-str-bigint");

    [Fact(DisplayName = "toString/symbol-tag-non-str-builtin.js")]
    public Task toString_symbol_tag_non_str_builtin() => ExecutionTestFromFile("toString/symbol-tag-non-str-builtin");

    [Fact(DisplayName = "toString/symbol-tag-non-str-proxy-function.js")]
    public Task toString_symbol_tag_non_str_proxy_function() => ExecutionTestFromFile("toString/symbol-tag-non-str-proxy-function");

    [Fact(DisplayName = "toString/symbol-tag-non-str.js")]
    public Task toString_symbol_tag_non_str() => ExecutionTestFromFile("toString/symbol-tag-non-str");

    [Fact(DisplayName = "toString/symbol-tag-override-bigint.js")]
    public Task toString_symbol_tag_override_bigint() => ExecutionTestFromFile("toString/symbol-tag-override-bigint");

    [Fact(DisplayName = "toString/symbol-tag-promise-builtin.js")]
    public Task toString_symbol_tag_promise_builtin() => ExecutionTestFromFile("toString/symbol-tag-promise-builtin");

    [Fact(DisplayName = "toString/symbol-tag-str.js")]
    public Task toString_symbol_tag_str() => ExecutionTestFromFile("toString/symbol-tag-str");

    [Fact(DisplayName = "toString/symbol-tag-weakset-builtin.js")]
    public Task toString_symbol_tag_weakset_builtin() => ExecutionTestFromFile("toString/symbol-tag-weakset-builtin");

    [Fact(DisplayName = "valueOf/S15.2.4.4_A10.js")]
    public Task valueOf_S15_2_4_4_A10() => ExecutionTestFromFile("valueOf/S15.2.4.4_A10");

    [Fact(DisplayName = "valueOf/S15.2.4.4_A11.js")]
    public Task valueOf_S15_2_4_4_A11() => ExecutionTestFromFile("valueOf/S15.2.4.4_A11");

    [Fact(DisplayName = "valueOf/S15.2.4.4_A12.js")]
    public Task valueOf_S15_2_4_4_A12() => ExecutionTestFromFile("valueOf/S15.2.4.4_A12");

    [Fact(DisplayName = "valueOf/S15.2.4.4_A13.js")]
    public Task valueOf_S15_2_4_4_A13() => ExecutionTestFromFile("valueOf/S15.2.4.4_A13");

    [Fact(DisplayName = "valueOf/S15.2.4.4_A14.js")]
    public Task valueOf_S15_2_4_4_A14() => ExecutionTestFromFile("valueOf/S15.2.4.4_A14");

    [Fact(DisplayName = "valueOf/S15.2.4.4_A15.js")]
    public Task valueOf_S15_2_4_4_A15() => ExecutionTestFromFile("valueOf/S15.2.4.4_A15");

    [Fact(DisplayName = "valueOf/S15.2.4.4_A1_T1.js")]
    public Task valueOf_S15_2_4_4_A1_T1() => ExecutionTestFromFile("valueOf/S15.2.4.4_A1_T1");

    [Fact(DisplayName = "valueOf/S15.2.4.4_A1_T2.js")]
    public Task valueOf_S15_2_4_4_A1_T2() => ExecutionTestFromFile("valueOf/S15.2.4.4_A1_T2");

    [Fact(DisplayName = "valueOf/S15.2.4.4_A1_T3.js")]
    public Task valueOf_S15_2_4_4_A1_T3() => ExecutionTestFromFile("valueOf/S15.2.4.4_A1_T3");

    [Fact(DisplayName = "valueOf/S15.2.4.4_A1_T4.js")]
    public Task valueOf_S15_2_4_4_A1_T4() => ExecutionTestFromFile("valueOf/S15.2.4.4_A1_T4");

    [Fact(DisplayName = "valueOf/S15.2.4.4_A1_T5.js")]
    public Task valueOf_S15_2_4_4_A1_T5() => ExecutionTestFromFile("valueOf/S15.2.4.4_A1_T5");

    [Fact(DisplayName = "valueOf/S15.2.4.4_A1_T6.js")]
    public Task valueOf_S15_2_4_4_A1_T6() => ExecutionTestFromFile("valueOf/S15.2.4.4_A1_T6");

    [Fact(DisplayName = "valueOf/S15.2.4.4_A1_T7.js")]
    public Task valueOf_S15_2_4_4_A1_T7() => ExecutionTestFromFile("valueOf/S15.2.4.4_A1_T7");

    [Fact(DisplayName = "valueOf/S15.2.4.4_A6.js")]
    public Task valueOf_S15_2_4_4_A6() => ExecutionTestFromFile("valueOf/S15.2.4.4_A6");

    [Fact(DisplayName = "valueOf/S15.2.4.4_A8.js")]
    public Task valueOf_S15_2_4_4_A8() => ExecutionTestFromFile("valueOf/S15.2.4.4_A8");

    [Fact(DisplayName = "valueOf/S15.2.4.4_A9.js")]
    public Task valueOf_S15_2_4_4_A9() => ExecutionTestFromFile("valueOf/S15.2.4.4_A9");

    [Fact(DisplayName = "valueOf/not-a-constructor.js")]
    public Task valueOf_not_a_constructor() => ExecutionTestFromFile("valueOf/not-a-constructor");
}
