using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.assign;

public class Test262BatchPort20260919Round3ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919Round3ExecutionTests() : base("built_ins.Object.assign") { }

    [Fact(DisplayName = "Source-Null-Undefined")]
    public Task Source_Null_Undefined()
        => ExecutionTestFromFile("Source-Null-Undefined");

    [Fact(DisplayName = "Source-Number-Boolen-Symbol")]
    public Task Source_Number_Boolen_Symbol()
        => ExecutionTestFromFile("Source-Number-Boolen-Symbol");

    [Fact(DisplayName = "Source-String")]
    public Task Source_String()
        => ExecutionTestFromFile("Source-String");

    [Fact(DisplayName = "Target-Boolean")]
    public Task Target_Boolean()
        => ExecutionTestFromFile("Target-Boolean");

    [Fact(DisplayName = "Target-Null")]
    public Task Target_Null()
        => ExecutionTestFromFile("Target-Null");

    [Fact(DisplayName = "Target-Number")]
    public Task Target_Number()
        => ExecutionTestFromFile("Target-Number");

    [Fact(DisplayName = "Target-Object")]
    public Task Target_Object()
        => ExecutionTestFromFile("Target-Object");

    [Fact(DisplayName = "Target-String")]
    public Task Target_String()
        => ExecutionTestFromFile("Target-String");

    [Fact(DisplayName = "Target-Symbol")]
    public Task Target_Symbol()
        => ExecutionTestFromFile("Target-Symbol");

    [Fact(DisplayName = "Target-Undefined")]
    public Task Target_Undefined()
        => ExecutionTestFromFile("Target-Undefined");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "source-get-attr-error")]
    public Task source_get_attr_error()
        => ExecutionTestFromFile("source-get-attr-error");

    [Fact(DisplayName = "source-non-enum")]
    public Task source_non_enum()
        => ExecutionTestFromFile("source-non-enum");

    [Fact(DisplayName = "strings-and-symbol-order")]
    public Task strings_and_symbol_order()
        => ExecutionTestFromFile("strings-and-symbol-order");

    [Fact(DisplayName = "target-is-frozen-accessor-property-set-succeeds")]
    public Task target_is_frozen_accessor_property_set_succeeds()
        => ExecutionTestFromFile("target-is-frozen-accessor-property-set-succeeds");

    [Fact(DisplayName = "target-is-frozen-data-property-set-throws")]
    public Task target_is_frozen_data_property_set_throws()
        => ExecutionTestFromFile("target-is-frozen-data-property-set-throws");

    [Fact(DisplayName = "target-is-non-extensible-existing-accessor-property")]
    public Task target_is_non_extensible_existing_accessor_property()
        => ExecutionTestFromFile("target-is-non-extensible-existing-accessor-property");

    [Fact(DisplayName = "target-is-non-extensible-existing-data-property")]
    public Task target_is_non_extensible_existing_data_property()
        => ExecutionTestFromFile("target-is-non-extensible-existing-data-property");

    [Fact(DisplayName = "target-is-non-extensible-property-creation-throws")]
    public Task target_is_non_extensible_property_creation_throws()
        => ExecutionTestFromFile("target-is-non-extensible-property-creation-throws");

    [Fact(DisplayName = "target-is-sealed-existing-accessor-property")]
    public Task target_is_sealed_existing_accessor_property()
        => ExecutionTestFromFile("target-is-sealed-existing-accessor-property");

    [Fact(DisplayName = "target-is-sealed-existing-data-property")]
    public Task target_is_sealed_existing_data_property()
        => ExecutionTestFromFile("target-is-sealed-existing-data-property");

    [Fact(DisplayName = "target-is-sealed-property-creation-throws")]
    public Task target_is_sealed_property_creation_throws()
        => ExecutionTestFromFile("target-is-sealed-property-creation-throws");

    [Fact(DisplayName = "target-set-not-writable")]
    public Task target_set_not_writable()
        => ExecutionTestFromFile("target-set-not-writable");

    [Fact(DisplayName = "target-set-user-error")]
    public Task target_set_user_error()
        => ExecutionTestFromFile("target-set-user-error");

}
