using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.WeakSet;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.WeakSet") { }

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "empty-iterable")]
    public Task empty_iterable()
        => ExecutionTestFromFile("empty-iterable");

    [Fact(DisplayName = "iterable-with-object-values")]
    public Task iterable_with_object_values()
        => ExecutionTestFromFile("iterable-with-object-values");

    [Fact(DisplayName = "get-add-method-failure")]
    public Task get_add_method_failure()
        => ExecutionTestFromFile("get-add-method-failure");

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

    [Fact(DisplayName = "properties-of-the-weakset-prototype-object")]
    public Task properties_of_the_weakset_prototype_object()
        => ExecutionTestFromFile("properties-of-the-weakset-prototype-object");

    [Fact(DisplayName = "prototype-of-weakset")]
    public Task prototype_of_weakset()
        => ExecutionTestFromFile("prototype-of-weakset");

    [Fact(DisplayName = "undefined-newtarget")]
    public Task undefined_newtarget()
        => ExecutionTestFromFile("undefined-newtarget");

    [Fact(DisplayName = "weakset")]
    public Task weakset()
        => ExecutionTestFromFile("weakset");
}
