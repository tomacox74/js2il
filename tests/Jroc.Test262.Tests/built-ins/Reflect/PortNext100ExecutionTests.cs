using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Reflect;

public class PortNext100ExecutionTests : DiskExecutionTestsBase
{
    public PortNext100ExecutionTests() : base("built_ins.Reflect") { }

    [Fact(DisplayName = "apply/apply")]
    public Task apply_apply()
        => ExecutionTestFromFile("apply/apply");

    [Fact(DisplayName = "apply/arguments-list-is-not-array-like-but-still-valid")]
    public Task apply_arguments_list_is_not_array_like_but_still_valid()
        => ExecutionTestFromFile("apply/arguments-list-is-not-array-like-but-still-valid");

    [Fact(DisplayName = "apply/target-is-not-callable-throws")]
    public Task apply_target_is_not_callable_throws()
        => ExecutionTestFromFile("apply/target-is-not-callable-throws");

    [Fact(DisplayName = "apply/return-target-call-result")]
    public Task apply_return_target_call_result()
        => ExecutionTestFromFile("apply/return-target-call-result");

    [Fact(DisplayName = "construct/construct")]
    public Task construct_construct()
        => ExecutionTestFromFile("construct/construct");

    [Fact(DisplayName = "construct/target-is-not-constructor-throws")]
    public Task construct_target_is_not_constructor_throws()
        => ExecutionTestFromFile("construct/target-is-not-constructor-throws");

    [Fact(DisplayName = "construct/return-with-newtarget-argument")]
    public Task construct_return_with_newtarget_argument()
        => ExecutionTestFromFile("construct/return-with-newtarget-argument");

    [Fact(DisplayName = "construct/use-arguments-list")]
    public Task construct_use_arguments_list()
        => ExecutionTestFromFile("construct/use-arguments-list");

    [Fact(DisplayName = "defineProperty/return-boolean")]
    public Task defineProperty_return_boolean()
        => ExecutionTestFromFile("defineProperty/return-boolean");

    [Fact(DisplayName = "defineProperty/target-is-not-object-throws")]
    public Task defineProperty_target_is_not_object_throws()
        => ExecutionTestFromFile("defineProperty/target-is-not-object-throws");

    [Fact(DisplayName = "deleteProperty/deleteProperty")]
    public Task deleteProperty_deleteProperty()
        => ExecutionTestFromFile("deleteProperty/deleteProperty");

    [Fact(DisplayName = "deleteProperty/return-boolean")]
    public Task deleteProperty_return_boolean()
        => ExecutionTestFromFile("deleteProperty/return-boolean");

    [Fact(DisplayName = "deleteProperty/target-is-not-object-throws")]
    public Task deleteProperty_target_is_not_object_throws()
        => ExecutionTestFromFile("deleteProperty/target-is-not-object-throws");

    [Fact(DisplayName = "get/get")]
    public Task get_get()
        => ExecutionTestFromFile("get/get");

    [Fact(DisplayName = "get/return-value-from-symbol-key")]
    public Task get_return_value_from_symbol_key()
        => ExecutionTestFromFile("get/return-value-from-symbol-key");

    [Fact(DisplayName = "get/target-is-not-object-throws")]
    public Task get_target_is_not_object_throws()
        => ExecutionTestFromFile("get/target-is-not-object-throws");

    [Fact(DisplayName = "getOwnPropertyDescriptor/return-from-accessor-descriptor")]
    public Task getOwnPropertyDescriptor_return_from_accessor_descriptor()
        => ExecutionTestFromFile("getOwnPropertyDescriptor/return-from-accessor-descriptor");

    [Fact(DisplayName = "getOwnPropertyDescriptor/symbol-property")]
    public Task getOwnPropertyDescriptor_symbol_property()
        => ExecutionTestFromFile("getOwnPropertyDescriptor/symbol-property");

    [Fact(DisplayName = "getOwnPropertyDescriptor/undefined-own-property")]
    public Task getOwnPropertyDescriptor_undefined_own_property()
        => ExecutionTestFromFile("getOwnPropertyDescriptor/undefined-own-property");

    [Fact(DisplayName = "getPrototypeOf/null-prototype")]
    public Task getPrototypeOf_null_prototype()
        => ExecutionTestFromFile("getPrototypeOf/null-prototype");

    [Fact(DisplayName = "getPrototypeOf/target-is-not-object-throws")]
    public Task getPrototypeOf_target_is_not_object_throws()
        => ExecutionTestFromFile("getPrototypeOf/target-is-not-object-throws");

    [Fact(DisplayName = "has/symbol-property")]
    public Task has_symbol_property()
        => ExecutionTestFromFile("has/symbol-property");

    [Fact(DisplayName = "has/target-is-not-object-throws")]
    public Task has_target_is_not_object_throws()
        => ExecutionTestFromFile("has/target-is-not-object-throws");

    [Fact(DisplayName = "isExtensible/target-is-not-object-throws")]
    public Task isExtensible_target_is_not_object_throws()
        => ExecutionTestFromFile("isExtensible/target-is-not-object-throws");

    [Fact(DisplayName = "ownKeys/return-empty-array")]
    public Task ownKeys_return_empty_array()
        => ExecutionTestFromFile("ownKeys/return-empty-array");

}
