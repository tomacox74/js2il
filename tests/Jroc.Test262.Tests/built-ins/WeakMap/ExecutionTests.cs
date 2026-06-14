using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakMap;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.WeakMap") { }

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "empty-iterable")]
    public Task empty_iterable()
        => ExecutionTestFromFile("empty-iterable");

    [Fact(DisplayName = "iterable-with-object-keys")]
    public Task iterable_with_object_keys()
        => ExecutionTestFromFile("iterable-with-object-keys");

    [Fact(DisplayName = "iterable-with-symbol-keys")]
    public Task iterable_with_symbol_keys()
        => ExecutionTestFromFile("iterable-with-symbol-keys");

    [Fact(DisplayName = "get-set-method-failure")]
    public Task get_set_method_failure()
        => ExecutionTestFromFile("get-set-method-failure");

    [Fact(DisplayName = "is-a-constructor")]
    public Task is_a_constructor()
        => ExecutionTestFromFile("is-a-constructor");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "no-iterable")]
    public Task no_iterable()
        => ExecutionTestFromFile("no-iterable");

    [Fact(DisplayName = "prototype-of-weakmap")]
    public Task prototype_of_weakmap()
        => ExecutionTestFromFile("prototype-of-weakmap");

    [Fact(DisplayName = "properties-of-map-instances")]
    public Task properties_of_map_instances()
        => ExecutionTestFromFile("properties-of-map-instances");

    [Fact(DisplayName = "properties-of-the-weakmap-prototype-object")]
    public Task properties_of_the_weakmap_prototype_object()
        => ExecutionTestFromFile("properties-of-the-weakmap-prototype-object");

    [Fact(DisplayName = "undefined-newtarget")]
    public Task undefined_newtarget()
        => ExecutionTestFromFile("undefined-newtarget");

    [Fact(DisplayName = "weakmap")]
    public Task weakmap()
        => ExecutionTestFromFile("weakmap");
}
