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

    [Fact(DisplayName = "get-set-method-failure")]
    public Task get_set_method_failure()
        => ExecutionTestFromFile("get-set-method-failure");

    [Fact(DisplayName = "is-a-constructor")]
    public Task is_a_constructor()
        => ExecutionTestFromFile("is-a-constructor");

    [Fact(DisplayName = "iterable-calls-set")]
    public Task iterable_calls_set()
        => ExecutionTestFromFile("iterable-calls-set");

    [Fact(DisplayName = "iterator-is-undefined-throws")]
    public Task iterator_is_undefined_throws()
        => ExecutionTestFromFile("iterator-is-undefined-throws");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

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

    [Fact(DisplayName = "prototype-of-map")]
    public Task prototype_of_map()
        => ExecutionTestFromFile("prototype-of-map");

    [Fact(DisplayName = "map")]
    public Task map()
        => ExecutionTestFromFile("map");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "properties-of-map-instances")]
    public Task properties_of_map_instances()
        => ExecutionTestFromFile("properties-of-map-instances");

    [Fact(DisplayName = "properties-of-the-map-prototype-object")]
    public Task properties_of_the_map_prototype_object()
        => ExecutionTestFromFile("properties-of-the-map-prototype-object");

    [Fact(DisplayName = "undefined-newtarget")]
    public Task undefined_newtarget()
        => ExecutionTestFromFile("undefined-newtarget");
    [Fact(DisplayName = "iterator-items-are-not-object-close-iterator")]
    public Task iterator_items_are_not_object_close_iterator()
        => ExecutionTestFromFile("iterator-items-are-not-object-close-iterator");

    [Fact(DisplayName = "iterator-items-are-not-object")]
    public Task iterator_items_are_not_object()
        => ExecutionTestFromFile("iterator-items-are-not-object");

    [Fact(DisplayName = "iterator-next-failure")]
    public Task iterator_next_failure()
        => ExecutionTestFromFile("iterator-next-failure");

    [Fact(DisplayName = "iterator-value-failure")]
    public Task iterator_value_failure()
        => ExecutionTestFromFile("iterator-value-failure");

    [Fact(DisplayName = "map-iterable-throws-when-set-is-not-callable")]
    public Task map_iterable_throws_when_set_is_not_callable()
        => ExecutionTestFromFile("map-iterable-throws-when-set-is-not-callable");

}
