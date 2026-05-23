using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Map;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Map") { }

    [Fact(DisplayName = "bigint-number-same-value")]
    public Task bigint_number_same_value()
        => ExecutionTestFromFile("bigint-number-same-value");

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "does-not-throw-when-set-is-not-callable")]
    public Task does_not_throw_when_set_is_not_callable()
        => ExecutionTestFromFile("does-not-throw-when-set-is-not-callable");

    [Fact(DisplayName = "map-iterable-empty-does-not-call-set")]
    public Task map_iterable_empty_does_not_call_set()
        => ExecutionTestFromFile("map-iterable-empty-does-not-call-set");

    [Fact(DisplayName = "map-iterable")]
    public Task map_iterable()
        => ExecutionTestFromFile("map-iterable");

    [Fact(DisplayName = "map-no-iterable-does-not-call-set")]
    public Task map_no_iterable_does_not_call_set()
        => ExecutionTestFromFile("map-no-iterable-does-not-call-set");

    [Fact(DisplayName = "map-no-iterable")]
    public Task map_no_iterable()
        => ExecutionTestFromFile("map-no-iterable");

    [Fact(DisplayName = "newtarget")]
    public Task newtarget()
        => ExecutionTestFromFile("newtarget");

    [Fact(DisplayName = "properties-of-map-instances")]
    public Task properties_of_map_instances()
        => ExecutionTestFromFile("properties-of-map-instances");

    [Fact(DisplayName = "properties-of-the-map-prototype-object")]
    public Task properties_of_the_map_prototype_object()
        => ExecutionTestFromFile("properties-of-the-map-prototype-object");
}
