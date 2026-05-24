using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.WeakMap;

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

    [Fact(DisplayName = "no-iterable")]
    public Task no_iterable()
        => ExecutionTestFromFile("no-iterable");

    [Fact(DisplayName = "properties-of-map-instances")]
    public Task properties_of_map_instances()
        => ExecutionTestFromFile("properties-of-map-instances");

    [Fact(DisplayName = "properties-of-the-weakmap-prototype-object")]
    public Task properties_of_the_weakmap_prototype_object()
        => ExecutionTestFromFile("properties-of-the-weakmap-prototype-object");
}
