using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Map.prototype.forEach;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Map.prototype.forEach") { }

    [Fact(DisplayName = "callback-parameters")]
    public Task callback_parameters()
        => ExecutionTestFromFile("callback-parameters");
    [Fact(DisplayName = "callback-result-is-abrupt")]
    public Task callback_result_is_abrupt()
        => ExecutionTestFromFile("callback-result-is-abrupt");

    [Fact(DisplayName = "callback-this-strict")]
    public Task callback_this_strict()
        => ExecutionTestFromFile("callback-this-strict");

    [Fact(DisplayName = "deleted-values-during-foreach")]
    public Task deleted_values_during_foreach()
        => ExecutionTestFromFile("deleted-values-during-foreach");

    [Fact(DisplayName = "does-not-have-mapdata-internal-slot-set")]
    public Task does_not_have_mapdata_internal_slot_set()
        => ExecutionTestFromFile("does-not-have-mapdata-internal-slot-set");

    [Fact(DisplayName = "does-not-have-mapdata-internal-slot-weakmap")]
    public Task does_not_have_mapdata_internal_slot_weakmap()
        => ExecutionTestFromFile("does-not-have-mapdata-internal-slot-weakmap");

    [Fact(DisplayName = "does-not-have-mapdata-internal-slot")]
    public Task does_not_have_mapdata_internal_slot()
        => ExecutionTestFromFile("does-not-have-mapdata-internal-slot");

    [Fact(DisplayName = "first-argument-is-not-callable")]
    public Task first_argument_is_not_callable()
        => ExecutionTestFromFile("first-argument-is-not-callable");

    [Fact(DisplayName = "iterates-in-key-insertion-order")]
    public Task iterates_in_key_insertion_order()
        => ExecutionTestFromFile("iterates-in-key-insertion-order");

    [Fact(DisplayName = "iterates-values-added-after-foreach-begins")]
    public Task iterates_values_added_after_foreach_begins()
        => ExecutionTestFromFile("iterates-values-added-after-foreach-begins");

    [Fact(DisplayName = "return-undefined")]
    public Task return_undefined()
        => ExecutionTestFromFile("return-undefined");

    [Fact(DisplayName = "second-parameter-as-callback-context")]
    public Task second_parameter_as_callback_context()
        => ExecutionTestFromFile("second-parameter-as-callback-context");

    [Fact(DisplayName = "this-not-object-throw")]
    public Task this_not_object_throw()
        => ExecutionTestFromFile("this-not-object-throw");

}
