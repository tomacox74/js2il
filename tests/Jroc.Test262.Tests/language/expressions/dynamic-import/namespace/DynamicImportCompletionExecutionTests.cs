namespace Jroc.Test262.Tests.language.expressions.dynamic_import.@namespace;

public sealed class DynamicImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public DynamicImportCompletionExecutionTests() : base("language.expressions.dynamic_import.namespace") { }

    [Fact(DisplayName = "await-ns-Symbol-toStringTag.js")]
    public Task await_ns_Symbol_toStringTag()
        => ExecutionTest("await-ns-Symbol-toStringTag");

    [Fact(DisplayName = "await-ns-define-own-property.js")]
    public Task await_ns_define_own_property()
        => ExecutionTest("await-ns-define-own-property");

    [Fact(DisplayName = "await-ns-delete-exported-init-no-strict.js")]
    public Task await_ns_delete_exported_init_no_strict()
        => ExecutionTest("await-ns-delete-exported-init-no-strict");

    [Fact(DisplayName = "await-ns-delete-exported-init-strict.js")]
    public Task await_ns_delete_exported_init_strict()
        => ExecutionTest("await-ns-delete-exported-init-strict");

    [Fact(DisplayName = "await-ns-delete-non-exported-no-strict.js")]
    public Task await_ns_delete_non_exported_no_strict()
        => ExecutionTest("await-ns-delete-non-exported-no-strict");

    [Fact(DisplayName = "await-ns-delete-non-exported-strict.js")]
    public Task await_ns_delete_non_exported_strict()
        => ExecutionTest("await-ns-delete-non-exported-strict");

    [Fact(DisplayName = "await-ns-extensible.js")]
    public Task await_ns_extensible()
        => ExecutionTest("await-ns-extensible");

    [Fact(DisplayName = "await-ns-get-nested-namespace-dflt-direct.js")]
    public Task await_ns_get_nested_namespace_dflt_direct()
        => ExecutionTest("await-ns-get-nested-namespace-dflt-direct");

    [Fact(DisplayName = "await-ns-get-nested-namespace-dflt-indirect.js")]
    public Task await_ns_get_nested_namespace_dflt_indirect()
        => ExecutionTest("await-ns-get-nested-namespace-dflt-indirect");

    [Fact(DisplayName = "await-ns-get-nested-namespace-props-nrml.js")]
    public Task await_ns_get_nested_namespace_props_nrml()
        => ExecutionTest("await-ns-get-nested-namespace-props-nrml");

    [Fact(DisplayName = "await-ns-get-own-property-str-found-init.js")]
    public Task await_ns_get_own_property_str_found_init()
        => ExecutionTest("await-ns-get-own-property-str-found-init");

    [Fact(DisplayName = "await-ns-get-own-property-str-not-found.js")]
    public Task await_ns_get_own_property_str_not_found()
        => ExecutionTest("await-ns-get-own-property-str-not-found");

    [Fact(DisplayName = "await-ns-get-own-property-sym.js")]
    public Task await_ns_get_own_property_sym()
        => ExecutionTest("await-ns-get-own-property-sym");

    [Fact(DisplayName = "await-ns-get-str-found.js")]
    public Task await_ns_get_str_found()
        => ExecutionTest("await-ns-get-str-found");

    [Fact(DisplayName = "await-ns-get-str-not-found.js")]
    public Task await_ns_get_str_not_found()
        => ExecutionTest("await-ns-get-str-not-found");

    [Fact(DisplayName = "await-ns-get-sym-found.js")]
    public Task await_ns_get_sym_found()
        => ExecutionTest("await-ns-get-sym-found");

    [Fact(DisplayName = "await-ns-get-sym-not-found.js")]
    public Task await_ns_get_sym_not_found()
        => ExecutionTest("await-ns-get-sym-not-found");

    [Fact(DisplayName = "await-ns-has-property-str-found-init.js")]
    public Task await_ns_has_property_str_found_init()
        => ExecutionTest("await-ns-has-property-str-found-init");

    [Fact(DisplayName = "await-ns-has-property-str-not-found.js")]
    public Task await_ns_has_property_str_not_found()
        => ExecutionTest("await-ns-has-property-str-not-found");

    [Fact(DisplayName = "await-ns-has-property-sym-found.js")]
    public Task await_ns_has_property_sym_found()
        => ExecutionTest("await-ns-has-property-sym-found");

    [Fact(DisplayName = "await-ns-has-property-sym-not-found.js")]
    public Task await_ns_has_property_sym_not_found()
        => ExecutionTest("await-ns-has-property-sym-not-found");

    [Fact(DisplayName = "await-ns-no-iterator.js")]
    public Task await_ns_no_iterator()
        => ExecutionTest("await-ns-no-iterator");

    [Fact(DisplayName = "await-ns-own-property-keys-sort.js")]
    public Task await_ns_own_property_keys_sort()
        => ExecutionTest("await-ns-own-property-keys-sort");

    [Fact(DisplayName = "await-ns-prevent-extensions-object.js")]
    public Task await_ns_prevent_extensions_object()
        => ExecutionTest("await-ns-prevent-extensions-object");

    [Fact(DisplayName = "await-ns-prevent-extensions-reflect.js")]
    public Task await_ns_prevent_extensions_reflect()
        => ExecutionTest("await-ns-prevent-extensions-reflect");

    [Fact(DisplayName = "await-ns-prop-descs.js")]
    public Task await_ns_prop_descs()
        => ExecutionTest("await-ns-prop-descs");

    [Fact(DisplayName = "await-ns-prototype.js")]
    public Task await_ns_prototype()
        => ExecutionTest("await-ns-prototype");

    [Fact(DisplayName = "await-ns-set-no-strict.js")]
    public Task await_ns_set_no_strict()
        => ExecutionTest("await-ns-set-no-strict");

    [Fact(DisplayName = "await-ns-set-prototype-of-null.js")]
    public Task await_ns_set_prototype_of_null()
        => ExecutionTest("await-ns-set-prototype-of-null");

    [Fact(DisplayName = "await-ns-set-prototype-of.js")]
    public Task await_ns_set_prototype_of()
        => ExecutionTest("await-ns-set-prototype-of");

    [Fact(DisplayName = "await-ns-set-same-values-no-strict.js")]
    public Task await_ns_set_same_values_no_strict()
        => ExecutionTest("await-ns-set-same-values-no-strict");

    [Fact(DisplayName = "await-ns-set-same-values-strict.js")]
    public Task await_ns_set_same_values_strict()
        => ExecutionTest("await-ns-set-same-values-strict");

    [Fact(DisplayName = "await-ns-set-strict.js")]
    public Task await_ns_set_strict()
        => ExecutionTest("await-ns-set-strict");

    [Fact(DisplayName = "default-property-not-set-own.js")]
    public Task default_property_not_set_own()
        => ExecutionTest("default-property-not-set-own");

    [Fact(DisplayName = "promise-then-ns-Symbol-toStringTag.js")]
    public Task promise_then_ns_Symbol_toStringTag()
        => ExecutionTest("promise-then-ns-Symbol-toStringTag");

    [Fact(DisplayName = "promise-then-ns-define-own-property.js")]
    public Task promise_then_ns_define_own_property()
        => ExecutionTest("promise-then-ns-define-own-property");

    [Fact(DisplayName = "promise-then-ns-delete-exported-init-no-strict.js")]
    public Task promise_then_ns_delete_exported_init_no_strict()
        => ExecutionTest("promise-then-ns-delete-exported-init-no-strict");

    [Fact(DisplayName = "promise-then-ns-delete-exported-init-strict.js")]
    public Task promise_then_ns_delete_exported_init_strict()
        => ExecutionTest("promise-then-ns-delete-exported-init-strict");

    [Fact(DisplayName = "promise-then-ns-delete-non-exported-no-strict.js")]
    public Task promise_then_ns_delete_non_exported_no_strict()
        => ExecutionTest("promise-then-ns-delete-non-exported-no-strict");

    [Fact(DisplayName = "promise-then-ns-delete-non-exported-strict.js")]
    public Task promise_then_ns_delete_non_exported_strict()
        => ExecutionTest("promise-then-ns-delete-non-exported-strict");

    [Fact(DisplayName = "promise-then-ns-extensible.js")]
    public Task promise_then_ns_extensible()
        => ExecutionTest("promise-then-ns-extensible");

    [Fact(DisplayName = "promise-then-ns-get-nested-namespace-dflt-direct.js")]
    public Task promise_then_ns_get_nested_namespace_dflt_direct()
        => ExecutionTest("promise-then-ns-get-nested-namespace-dflt-direct");

    [Fact(DisplayName = "promise-then-ns-get-nested-namespace-dflt-indirect.js")]
    public Task promise_then_ns_get_nested_namespace_dflt_indirect()
        => ExecutionTest("promise-then-ns-get-nested-namespace-dflt-indirect");

    [Fact(DisplayName = "promise-then-ns-get-nested-namespace-props-nrml.js")]
    public Task promise_then_ns_get_nested_namespace_props_nrml()
        => ExecutionTest("promise-then-ns-get-nested-namespace-props-nrml");

    [Fact(DisplayName = "promise-then-ns-get-own-property-str-found-init.js")]
    public Task promise_then_ns_get_own_property_str_found_init()
        => ExecutionTest("promise-then-ns-get-own-property-str-found-init");

    [Fact(DisplayName = "promise-then-ns-get-own-property-str-not-found.js")]
    public Task promise_then_ns_get_own_property_str_not_found()
        => ExecutionTest("promise-then-ns-get-own-property-str-not-found");

    [Fact(DisplayName = "promise-then-ns-get-own-property-sym.js")]
    public Task promise_then_ns_get_own_property_sym()
        => ExecutionTest("promise-then-ns-get-own-property-sym");

    [Fact(DisplayName = "promise-then-ns-get-str-found.js")]
    public Task promise_then_ns_get_str_found()
        => ExecutionTest("promise-then-ns-get-str-found");

    [Fact(DisplayName = "promise-then-ns-get-str-not-found.js")]
    public Task promise_then_ns_get_str_not_found()
        => ExecutionTest("promise-then-ns-get-str-not-found");

    [Fact(DisplayName = "promise-then-ns-get-sym-found.js")]
    public Task promise_then_ns_get_sym_found()
        => ExecutionTest("promise-then-ns-get-sym-found");

    [Fact(DisplayName = "promise-then-ns-get-sym-not-found.js")]
    public Task promise_then_ns_get_sym_not_found()
        => ExecutionTest("promise-then-ns-get-sym-not-found");

    [Fact(DisplayName = "promise-then-ns-has-property-str-found-init.js")]
    public Task promise_then_ns_has_property_str_found_init()
        => ExecutionTest("promise-then-ns-has-property-str-found-init");

    [Fact(DisplayName = "promise-then-ns-has-property-str-not-found.js")]
    public Task promise_then_ns_has_property_str_not_found()
        => ExecutionTest("promise-then-ns-has-property-str-not-found");

    [Fact(DisplayName = "promise-then-ns-has-property-sym-found.js")]
    public Task promise_then_ns_has_property_sym_found()
        => ExecutionTest("promise-then-ns-has-property-sym-found");

    [Fact(DisplayName = "promise-then-ns-has-property-sym-not-found.js")]
    public Task promise_then_ns_has_property_sym_not_found()
        => ExecutionTest("promise-then-ns-has-property-sym-not-found");

    [Fact(DisplayName = "promise-then-ns-no-iterator.js")]
    public Task promise_then_ns_no_iterator()
        => ExecutionTest("promise-then-ns-no-iterator");

    [Fact(DisplayName = "promise-then-ns-own-property-keys-sort.js")]
    public Task promise_then_ns_own_property_keys_sort()
        => ExecutionTest("promise-then-ns-own-property-keys-sort");

    [Fact(DisplayName = "promise-then-ns-prevent-extensions-object.js")]
    public Task promise_then_ns_prevent_extensions_object()
        => ExecutionTest("promise-then-ns-prevent-extensions-object");

    [Fact(DisplayName = "promise-then-ns-prevent-extensions-reflect.js")]
    public Task promise_then_ns_prevent_extensions_reflect()
        => ExecutionTest("promise-then-ns-prevent-extensions-reflect");

    [Fact(DisplayName = "promise-then-ns-prop-descs.js")]
    public Task promise_then_ns_prop_descs()
        => ExecutionTest("promise-then-ns-prop-descs");

    [Fact(DisplayName = "promise-then-ns-prototype.js")]
    public Task promise_then_ns_prototype()
        => ExecutionTest("promise-then-ns-prototype");

    [Fact(DisplayName = "promise-then-ns-set-no-strict.js")]
    public Task promise_then_ns_set_no_strict()
        => ExecutionTest("promise-then-ns-set-no-strict");

    [Fact(DisplayName = "promise-then-ns-set-prototype-of-null.js")]
    public Task promise_then_ns_set_prototype_of_null()
        => ExecutionTest("promise-then-ns-set-prototype-of-null");

    [Fact(DisplayName = "promise-then-ns-set-prototype-of.js")]
    public Task promise_then_ns_set_prototype_of()
        => ExecutionTest("promise-then-ns-set-prototype-of");

    [Fact(DisplayName = "promise-then-ns-set-same-values-no-strict.js")]
    public Task promise_then_ns_set_same_values_no_strict()
        => ExecutionTest("promise-then-ns-set-same-values-no-strict");

    [Fact(DisplayName = "promise-then-ns-set-same-values-strict.js")]
    public Task promise_then_ns_set_same_values_strict()
        => ExecutionTest("promise-then-ns-set-same-values-strict");

    [Fact(DisplayName = "promise-then-ns-set-strict.js")]
    public Task promise_then_ns_set_strict()
        => ExecutionTest("promise-then-ns-set-strict");
}
