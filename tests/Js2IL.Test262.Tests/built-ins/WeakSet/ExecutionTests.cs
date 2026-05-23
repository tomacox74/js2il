using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.WeakSet;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.WeakSet") { }

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "empty-iterable", Skip = "Tracked by #1093: JS2IL does not yet pass this advanced test262 scenario.")]
    public Task empty_iterable()
        => ExecutionTestFromFile("empty-iterable");

    [Fact(DisplayName = "iterable-with-object-values", Skip = "Tracked by #1093: JS2IL does not yet pass this advanced test262 scenario.")]
    public Task iterable_with_object_values()
        => ExecutionTestFromFile("iterable-with-object-values");

    [Fact(DisplayName = "no-iterable", Skip = "Tracked by #1093: JS2IL does not yet pass this advanced test262 scenario.")]
    public Task no_iterable()
        => ExecutionTestFromFile("no-iterable");

    [Fact(DisplayName = "properties-of-the-weakset-prototype-object")]
    public Task properties_of_the_weakset_prototype_object()
        => ExecutionTestFromFile("properties-of-the-weakset-prototype-object");
}
