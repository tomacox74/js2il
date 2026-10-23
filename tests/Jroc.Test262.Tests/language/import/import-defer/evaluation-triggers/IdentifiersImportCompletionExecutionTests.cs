namespace Jroc.Test262.Tests.language.import.import_defer.evaluation_triggers;

public sealed class IdentifiersImportCompletionExecutionTests : DiskExecutionTestsBase
{
    public IdentifiersImportCompletionExecutionTests() : base("language.import.import_defer.evaluation_triggers") { }

    [Fact(DisplayName = "ignore-exported-then-defineOwnProperty.js")]
    public Task ignore_exported_then_defineOwnProperty()
        => ExecutionTest("ignore-exported-then-defineOwnProperty");

    [Fact(DisplayName = "ignore-exported-then-delete.js")]
    public Task ignore_exported_then_delete()
        => ExecutionTest("ignore-exported-then-delete");

    [Fact(DisplayName = "ignore-exported-then-get-in-prototype.js")]
    public Task ignore_exported_then_get_in_prototype()
        => ExecutionTest("ignore-exported-then-get-in-prototype");

    [Fact(DisplayName = "ignore-exported-then-get.js")]
    public Task ignore_exported_then_get()
        => ExecutionTest("ignore-exported-then-get");

    [Fact(DisplayName = "ignore-exported-then-getOwnProperty.js")]
    public Task ignore_exported_then_getOwnProperty()
        => ExecutionTest("ignore-exported-then-getOwnProperty");

    [Fact(DisplayName = "ignore-exported-then-hasProperty-in-prototype.js")]
    public Task ignore_exported_then_hasProperty_in_prototype()
        => ExecutionTest("ignore-exported-then-hasProperty-in-prototype");

    [Fact(DisplayName = "ignore-exported-then-hasProperty.js")]
    public Task ignore_exported_then_hasProperty()
        => ExecutionTest("ignore-exported-then-hasProperty");

    [Fact(DisplayName = "ignore-exported-then-super-get.js")]
    public Task ignore_exported_then_super_get()
        => ExecutionTest("ignore-exported-then-super-get");

    [Fact(DisplayName = "ignore-exported-then-super-property-define.js")]
    public Task ignore_exported_then_super_property_define()
        => ExecutionTest("ignore-exported-then-super-property-define");

    [Fact(DisplayName = "ignore-exported-then-super-property-set-exported.js")]
    public Task ignore_exported_then_super_property_set_exported()
        => ExecutionTest("ignore-exported-then-super-property-set-exported");

    [Fact(DisplayName = "ignore-getPrototypeOf.js")]
    public Task ignore_getPrototypeOf()
        => ExecutionTest("ignore-getPrototypeOf");

    [Fact(DisplayName = "ignore-isExtensible.js")]
    public Task ignore_isExtensible()
        => ExecutionTest("ignore-isExtensible");

    [Fact(DisplayName = "ignore-not-exported-then-defineOwnProperty.js")]
    public Task ignore_not_exported_then_defineOwnProperty()
        => ExecutionTest("ignore-not-exported-then-defineOwnProperty");

    [Fact(DisplayName = "ignore-not-exported-then-delete.js")]
    public Task ignore_not_exported_then_delete()
        => ExecutionTest("ignore-not-exported-then-delete");

    [Fact(DisplayName = "ignore-not-exported-then-get-in-prototype.js")]
    public Task ignore_not_exported_then_get_in_prototype()
        => ExecutionTest("ignore-not-exported-then-get-in-prototype");

    [Fact(DisplayName = "ignore-not-exported-then-get.js")]
    public Task ignore_not_exported_then_get()
        => ExecutionTest("ignore-not-exported-then-get");

    [Fact(DisplayName = "ignore-not-exported-then-getOwnProperty.js")]
    public Task ignore_not_exported_then_getOwnProperty()
        => ExecutionTest("ignore-not-exported-then-getOwnProperty");

    [Fact(DisplayName = "ignore-not-exported-then-hasProperty-in-prototype.js")]
    public Task ignore_not_exported_then_hasProperty_in_prototype()
        => ExecutionTest("ignore-not-exported-then-hasProperty-in-prototype");

    [Fact(DisplayName = "ignore-not-exported-then-hasProperty.js")]
    public Task ignore_not_exported_then_hasProperty()
        => ExecutionTest("ignore-not-exported-then-hasProperty");

    [Fact(DisplayName = "ignore-not-exported-then-super-get.js")]
    public Task ignore_not_exported_then_super_get()
        => ExecutionTest("ignore-not-exported-then-super-get");

    [Fact(DisplayName = "ignore-not-exported-then-super-property-define.js")]
    public Task ignore_not_exported_then_super_property_define()
        => ExecutionTest("ignore-not-exported-then-super-property-define");

    [Fact(DisplayName = "ignore-not-exported-then-super-property-set-exported.js")]
    public Task ignore_not_exported_then_super_property_set_exported()
        => ExecutionTest("ignore-not-exported-then-super-property-set-exported");

    [Fact(DisplayName = "ignore-preventExtensions.js")]
    public Task ignore_preventExtensions()
        => ExecutionTest("ignore-preventExtensions");

    [Fact(DisplayName = "ignore-private-name-access.js")]
    public Task ignore_private_name_access()
        => ExecutionTest("ignore-private-name-access");

    [Fact(DisplayName = "ignore-set-string-exported.js")]
    public Task ignore_set_string_exported()
        => ExecutionTest("ignore-set-string-exported");

    [Fact(DisplayName = "ignore-set-string-not-exported.js")]
    public Task ignore_set_string_not_exported()
        => ExecutionTest("ignore-set-string-not-exported");

    [Fact(DisplayName = "ignore-setPrototypeOf.js")]
    public Task ignore_setPrototypeOf()
        => ExecutionTest("ignore-setPrototypeOf");

    [Fact(DisplayName = "ignore-symbol-other-defineOwnProperty.js")]
    public Task ignore_symbol_other_defineOwnProperty()
        => ExecutionTest("ignore-symbol-other-defineOwnProperty");

    [Fact(DisplayName = "ignore-symbol-other-delete.js")]
    public Task ignore_symbol_other_delete()
        => ExecutionTest("ignore-symbol-other-delete");

    [Fact(DisplayName = "ignore-symbol-other-get-in-prototype.js")]
    public Task ignore_symbol_other_get_in_prototype()
        => ExecutionTest("ignore-symbol-other-get-in-prototype");

    [Fact(DisplayName = "ignore-symbol-other-get.js")]
    public Task ignore_symbol_other_get()
        => ExecutionTest("ignore-symbol-other-get");

    [Fact(DisplayName = "ignore-symbol-other-getOwnProperty.js")]
    public Task ignore_symbol_other_getOwnProperty()
        => ExecutionTest("ignore-symbol-other-getOwnProperty");

    [Fact(DisplayName = "ignore-symbol-other-hasProperty-in-prototype.js")]
    public Task ignore_symbol_other_hasProperty_in_prototype()
        => ExecutionTest("ignore-symbol-other-hasProperty-in-prototype");

    [Fact(DisplayName = "ignore-symbol-other-hasProperty.js")]
    public Task ignore_symbol_other_hasProperty()
        => ExecutionTest("ignore-symbol-other-hasProperty");

    [Fact(DisplayName = "ignore-symbol-other-super-get.js")]
    public Task ignore_symbol_other_super_get()
        => ExecutionTest("ignore-symbol-other-super-get");

    [Fact(DisplayName = "ignore-symbol-other-super-property-define.js")]
    public Task ignore_symbol_other_super_property_define()
        => ExecutionTest("ignore-symbol-other-super-property-define");

    [Fact(DisplayName = "ignore-symbol-other-super-property-set-exported.js")]
    public Task ignore_symbol_other_super_property_set_exported()
        => ExecutionTest("ignore-symbol-other-super-property-set-exported");

    [Fact(DisplayName = "ignore-symbol-toStringTag-defineOwnProperty.js")]
    public Task ignore_symbol_toStringTag_defineOwnProperty()
        => ExecutionTest("ignore-symbol-toStringTag-defineOwnProperty");

    [Fact(DisplayName = "ignore-symbol-toStringTag-delete.js")]
    public Task ignore_symbol_toStringTag_delete()
        => ExecutionTest("ignore-symbol-toStringTag-delete");

    [Fact(DisplayName = "ignore-symbol-toStringTag-get-in-prototype.js")]
    public Task ignore_symbol_toStringTag_get_in_prototype()
        => ExecutionTest("ignore-symbol-toStringTag-get-in-prototype");

    [Fact(DisplayName = "ignore-symbol-toStringTag-get.js")]
    public Task ignore_symbol_toStringTag_get()
        => ExecutionTest("ignore-symbol-toStringTag-get");

    [Fact(DisplayName = "ignore-symbol-toStringTag-getOwnProperty.js")]
    public Task ignore_symbol_toStringTag_getOwnProperty()
        => ExecutionTest("ignore-symbol-toStringTag-getOwnProperty");

    [Fact(DisplayName = "ignore-symbol-toStringTag-hasProperty-in-prototype.js")]
    public Task ignore_symbol_toStringTag_hasProperty_in_prototype()
        => ExecutionTest("ignore-symbol-toStringTag-hasProperty-in-prototype");

    [Fact(DisplayName = "ignore-symbol-toStringTag-hasProperty.js")]
    public Task ignore_symbol_toStringTag_hasProperty()
        => ExecutionTest("ignore-symbol-toStringTag-hasProperty");

    [Fact(DisplayName = "ignore-symbol-toStringTag-super-get.js")]
    public Task ignore_symbol_toStringTag_super_get()
        => ExecutionTest("ignore-symbol-toStringTag-super-get");

    [Fact(DisplayName = "ignore-symbol-toStringTag-super-property-define.js")]
    public Task ignore_symbol_toStringTag_super_property_define()
        => ExecutionTest("ignore-symbol-toStringTag-super-property-define");

    [Fact(DisplayName = "ignore-symbol-toStringTag-super-property-set-exported.js")]
    public Task ignore_symbol_toStringTag_super_property_set_exported()
        => ExecutionTest("ignore-symbol-toStringTag-super-property-set-exported");

    [Fact(DisplayName = "trigger-exported-string-defineOwnProperty.js")]
    public Task trigger_exported_string_defineOwnProperty()
        => ExecutionTest("trigger-exported-string-defineOwnProperty");

    [Fact(DisplayName = "trigger-exported-string-delete.js")]
    public Task trigger_exported_string_delete()
        => ExecutionTest("trigger-exported-string-delete");

    [Fact(DisplayName = "trigger-exported-string-get-in-prototype.js")]
    public Task trigger_exported_string_get_in_prototype()
        => ExecutionTest("trigger-exported-string-get-in-prototype");

    [Fact(DisplayName = "trigger-exported-string-get.js")]
    public Task trigger_exported_string_get()
        => ExecutionTest("trigger-exported-string-get");

    [Fact(DisplayName = "trigger-exported-string-getOwnProperty.js")]
    public Task trigger_exported_string_getOwnProperty()
        => ExecutionTest("trigger-exported-string-getOwnProperty");

    [Fact(DisplayName = "trigger-exported-string-hasProperty-in-prototype.js")]
    public Task trigger_exported_string_hasProperty_in_prototype()
        => ExecutionTest("trigger-exported-string-hasProperty-in-prototype");

    [Fact(DisplayName = "trigger-exported-string-hasProperty.js")]
    public Task trigger_exported_string_hasProperty()
        => ExecutionTest("trigger-exported-string-hasProperty");

    [Fact(DisplayName = "trigger-exported-string-super-get.js")]
    public Task trigger_exported_string_super_get()
        => ExecutionTest("trigger-exported-string-super-get");

    [Fact(DisplayName = "trigger-exported-string-super-property-define.js")]
    public Task trigger_exported_string_super_property_define()
        => ExecutionTest("trigger-exported-string-super-property-define");

    [Fact(DisplayName = "trigger-exported-string-super-property-set-exported.js")]
    public Task trigger_exported_string_super_property_set_exported()
        => ExecutionTest("trigger-exported-string-super-property-set-exported");

    [Fact(DisplayName = "trigger-not-exported-string-defineOwnProperty.js")]
    public Task trigger_not_exported_string_defineOwnProperty()
        => ExecutionTest("trigger-not-exported-string-defineOwnProperty");

    [Fact(DisplayName = "trigger-not-exported-string-delete.js")]
    public Task trigger_not_exported_string_delete()
        => ExecutionTest("trigger-not-exported-string-delete");

    [Fact(DisplayName = "trigger-not-exported-string-get-in-prototype.js")]
    public Task trigger_not_exported_string_get_in_prototype()
        => ExecutionTest("trigger-not-exported-string-get-in-prototype");

    [Fact(DisplayName = "trigger-not-exported-string-get.js")]
    public Task trigger_not_exported_string_get()
        => ExecutionTest("trigger-not-exported-string-get");

    [Fact(DisplayName = "trigger-not-exported-string-getOwnProperty.js")]
    public Task trigger_not_exported_string_getOwnProperty()
        => ExecutionTest("trigger-not-exported-string-getOwnProperty");

    [Fact(DisplayName = "trigger-not-exported-string-hasProperty-in-prototype.js")]
    public Task trigger_not_exported_string_hasProperty_in_prototype()
        => ExecutionTest("trigger-not-exported-string-hasProperty-in-prototype");

    [Fact(DisplayName = "trigger-not-exported-string-hasProperty.js")]
    public Task trigger_not_exported_string_hasProperty()
        => ExecutionTest("trigger-not-exported-string-hasProperty");

    [Fact(DisplayName = "trigger-not-exported-string-super-get.js")]
    public Task trigger_not_exported_string_super_get()
        => ExecutionTest("trigger-not-exported-string-super-get");

    [Fact(DisplayName = "trigger-not-exported-string-super-property-define.js")]
    public Task trigger_not_exported_string_super_property_define()
        => ExecutionTest("trigger-not-exported-string-super-property-define");

    [Fact(DisplayName = "trigger-not-exported-string-super-property-set-exported.js")]
    public Task trigger_not_exported_string_super_property_set_exported()
        => ExecutionTest("trigger-not-exported-string-super-property-set-exported");

    [Fact(DisplayName = "trigger-ownPropertyKey-names.js")]
    public Task trigger_ownPropertyKey_names()
        => ExecutionTest("trigger-ownPropertyKey-names");

    [Fact(DisplayName = "trigger-ownPropertyKeys-symbols.js")]
    public Task trigger_ownPropertyKeys_symbols()
        => ExecutionTest("trigger-ownPropertyKeys-symbols");

    [Fact(DisplayName = "trigger-ownPropertyKeys.js")]
    public Task trigger_ownPropertyKeys()
        => ExecutionTest("trigger-ownPropertyKeys");
}
