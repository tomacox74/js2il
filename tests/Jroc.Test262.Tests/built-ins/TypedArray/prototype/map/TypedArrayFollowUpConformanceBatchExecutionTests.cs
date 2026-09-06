using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.map;

public class TypedArrayFollowUpConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayFollowUpConformanceBatchExecutionTests() : base("built_ins.TypedArray.prototype.map") { }

    [Fact(DisplayName = "arraylength-internal.js")]
    public Task arraylength_internal() => ExecutionTestFromFile("arraylength-internal");

    [Fact(DisplayName = "callbackfn-arguments-with-thisarg.js")]
    public Task callbackfn_arguments_with_thisarg() => ExecutionTestFromFile("callbackfn-arguments-with-thisarg");

    [Fact(DisplayName = "callbackfn-arguments-without-thisarg.js")]
    public Task callbackfn_arguments_without_thisarg() => ExecutionTestFromFile("callbackfn-arguments-without-thisarg");

    [Fact(DisplayName = "callbackfn-is-not-callable.js")]
    public Task callbackfn_is_not_callable() => ExecutionTestFromFile("callbackfn-is-not-callable");

    [Fact(DisplayName = "callbackfn-no-interaction-over-non-integer-properties.js")]
    public Task callbackfn_no_interaction_over_non_integer_properties() => ExecutionTestFromFile("callbackfn-no-interaction-over-non-integer-properties");

    [Fact(DisplayName = "callbackfn-not-called-on-empty.js")]
    public Task callbackfn_not_called_on_empty() => ExecutionTestFromFile("callbackfn-not-called-on-empty");

    [Fact(DisplayName = "callbackfn-return-affects-returned-object.js")]
    public Task callbackfn_return_affects_returned_object() => ExecutionTestFromFile("callbackfn-return-affects-returned-object");

    [Fact(DisplayName = "callbackfn-return-does-not-change-instance.js")]
    public Task callbackfn_return_does_not_change_instance() => ExecutionTestFromFile("callbackfn-return-does-not-change-instance");

    [Fact(DisplayName = "callbackfn-return-does-not-copy-non-integer-properties.js")]
    public Task callbackfn_return_does_not_copy_non_integer_properties() => ExecutionTestFromFile("callbackfn-return-does-not-copy-non-integer-properties");

    [Fact(DisplayName = "callbackfn-returns-abrupt.js")]
    public Task callbackfn_returns_abrupt() => ExecutionTestFromFile("callbackfn-returns-abrupt");

    [Fact(DisplayName = "callbackfn-set-value-during-interaction.js")]
    public Task callbackfn_set_value_during_interaction() => ExecutionTestFromFile("callbackfn-set-value-during-interaction");

    [Fact(DisplayName = "callbackfn-this.js")]
    public Task callbackfn_this() => ExecutionTestFromFile("callbackfn-this");

    [Fact(DisplayName = "return-new-typedarray-conversion-operation-consistent-nan.js")]
    public Task return_new_typedarray_conversion_operation_consistent_nan() => ExecutionTestFromFile("return-new-typedarray-conversion-operation-consistent-nan");

    [Fact(DisplayName = "return-new-typedarray-from-empty-length.js")]
    public Task return_new_typedarray_from_empty_length() => ExecutionTestFromFile("return-new-typedarray-from-empty-length");

    [Fact(DisplayName = "return-new-typedarray-from-positive-length.js")]
    public Task return_new_typedarray_from_positive_length() => ExecutionTestFromFile("return-new-typedarray-from-positive-length");

    [Fact(DisplayName = "speciesctor-get-ctor-abrupt.js")]
    public Task speciesctor_get_ctor_abrupt() => ExecutionTestFromFile("speciesctor-get-ctor-abrupt");

    [Fact(DisplayName = "speciesctor-get-species-abrupt.js")]
    public Task speciesctor_get_species_abrupt() => ExecutionTestFromFile("speciesctor-get-species-abrupt");

    [Fact(DisplayName = "this-is-not-object.js")]
    public Task this_is_not_object() => ExecutionTestFromFile("this-is-not-object");

    [Fact(DisplayName = "this-is-not-typedarray-instance.js")]
    public Task this_is_not_typedarray_instance() => ExecutionTestFromFile("this-is-not-typedarray-instance");

    [Fact(DisplayName = "values-are-not-cached.js")]
    public Task values_are_not_cached() => ExecutionTestFromFile("values-are-not-cached");

}
