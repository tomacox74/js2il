using Jroc.Test262.Tests.language.modules;

namespace Jroc.Test262.Tests.language.module_code.namespace_.internals;

public class ExecutionTests : FileSystemExecutionTestsBase
{
    public ExecutionTests() : base("language/module-code/namespace/internals", "language.module_code.namespace_.internals") { }

    [Fact(DisplayName = "define-own-property")]
    public Task define_own_property() => ExecutionTest("define-own-property");
    [Fact(DisplayName = "delete-exported-init")]
    public Task delete_exported_init() => ExecutionTest("delete-exported-init");
    [Fact(DisplayName = "delete-exported-uninit")]
    public Task delete_exported_uninit() => ExecutionTest("delete-exported-uninit");
    [Fact(DisplayName = "delete-non-exported")]
    public Task delete_non_exported() => ExecutionTest("delete-non-exported");
    [Fact(DisplayName = "get-nested-namespace-dflt-skip")]
    public Task get_nested_namespace_dflt_skip() => ExecutionTest("get-nested-namespace-dflt-skip");
    [Fact(DisplayName = "get-nested-namespace-props-nrml")]
    public Task get_nested_namespace_props_nrml() => ExecutionTest("get-nested-namespace-props-nrml");
    [Fact(DisplayName = "get-own-property-str-found-init")]
    public Task get_own_property_str_found_init() => ExecutionTest("get-own-property-str-found-init");
    [Fact(DisplayName = "get-own-property-str-found-uninit")]
    public Task get_own_property_str_found_uninit() => ExecutionTest("get-own-property-str-found-uninit");
    [Fact(DisplayName = "get-own-property-str-not-found")]
    public Task get_own_property_str_not_found() => ExecutionTest("get-own-property-str-not-found");
    [Fact(DisplayName = "get-own-property-sym")]
    public Task get_own_property_sym() => ExecutionTest("get-own-property-sym");
    [Fact(DisplayName = "get-prototype-of")]
    public Task get_prototype_of() => ExecutionTest("get-prototype-of");
    [Fact(DisplayName = "get-str-found-init")]
    public Task get_str_found_init() => ExecutionTest("get-str-found-init");
    [Fact(DisplayName = "get-str-found-uninit")]
    public Task get_str_found_uninit() => ExecutionTest("get-str-found-uninit");
    [Fact(DisplayName = "get-str-initialize")]
    public Task get_str_initialize() => ExecutionTest("get-str-initialize");
    [Fact(DisplayName = "get-str-not-found")]
    public Task get_str_not_found() => ExecutionTest("get-str-not-found");
    [Fact(DisplayName = "get-str-update")]
    public Task get_str_update() => ExecutionTest("get-str-update");
    [Fact(DisplayName = "get-sym-found")]
    public Task get_sym_found() => ExecutionTest("get-sym-found");
    [Fact(DisplayName = "get-sym-not-found")]
    public Task get_sym_not_found() => ExecutionTest("get-sym-not-found");
    [Fact(DisplayName = "has-property-str-found-init")]
    public Task has_property_str_found_init() => ExecutionTest("has-property-str-found-init");
    [Fact(DisplayName = "has-property-str-found-uninit")]
    public Task has_property_str_found_uninit() => ExecutionTest("has-property-str-found-uninit");
    [Fact(DisplayName = "has-property-str-not-found")]
    public Task has_property_str_not_found() => ExecutionTest("has-property-str-not-found");
    [Fact(DisplayName = "has-property-sym-found")]
    public Task has_property_sym_found() => ExecutionTest("has-property-sym-found");
    [Fact(DisplayName = "has-property-sym-not-found")]
    public Task has_property_sym_not_found() => ExecutionTest("has-property-sym-not-found");
    [Fact(DisplayName = "is-extensible")]
    public Task is_extensible() => ExecutionTest("is-extensible");
    [Fact(DisplayName = "object-hasOwnProperty-binding-uninit")]
    public Task object_hasOwnProperty_binding_uninit() => ExecutionTest("object-hasOwnProperty-binding-uninit");
    [Fact(DisplayName = "object-keys-binding-uninit")]
    public Task object_keys_binding_uninit() => ExecutionTest("object-keys-binding-uninit");
    [Fact(DisplayName = "object-propertyIsEnumerable-binding-uninit")]
    public Task object_propertyIsEnumerable_binding_uninit() => ExecutionTest("object-propertyIsEnumerable-binding-uninit");
    [Fact(DisplayName = "own-property-keys-binding-types")]
    public Task own_property_keys_binding_types() => ExecutionTest("own-property-keys-binding-types");
    [Fact(DisplayName = "own-property-keys-sort")]
    public Task own_property_keys_sort() => ExecutionTest("own-property-keys-sort");
    [Fact(DisplayName = "prevent-extensions")]
    public Task prevent_extensions() => ExecutionTest("prevent-extensions");
    [Fact(DisplayName = "set-prototype-of-null")]
    public Task set_prototype_of_null() => ExecutionTest("set-prototype-of-null");
    [Fact(DisplayName = "set-prototype-of")]
    public Task set_prototype_of() => ExecutionTest("set-prototype-of");
    [Fact(DisplayName = "set")]
    public Task set() => ExecutionTest("set");
    [Fact(DisplayName = "super-access-to-tdz-binding")]
    public Task super_access_to_tdz_binding() => ExecutionTest("super-access-to-tdz-binding");
    [Fact(DisplayName = "super-set-to-tdz-binding-with-accessor")]
    public Task super_set_to_tdz_binding_with_accessor() => ExecutionTest("super-set-to-tdz-binding-with-accessor");
}
