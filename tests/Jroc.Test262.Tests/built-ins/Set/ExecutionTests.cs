using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Set") { }

    [Fact(DisplayName = "bigint-number-same-value")]
    public Task bigint_number_same_value()
        => ExecutionTestFromFile("bigint-number-same-value");

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "is-a-constructor")]
    public Task is_a_constructor()
        => ExecutionTestFromFile("is-a-constructor");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "properties-of-the-set-prototype-object")]
    public Task properties_of_the_set_prototype_object()
        => ExecutionTestFromFile("properties-of-the-set-prototype-object");

    [Fact(DisplayName = "prototype-of-set")]
    public Task prototype_of_set()
        => ExecutionTestFromFile("prototype-of-set");

    [Fact(DisplayName = "set")]
    public Task set()
        => ExecutionTestFromFile("set");

    [Fact(DisplayName = "set-does-not-throw-when-add-is-not-callable")]
    public Task set_does_not_throw_when_add_is_not_callable()
        => ExecutionTestFromFile("set-does-not-throw-when-add-is-not-callable");

    [Fact(DisplayName = "set-get-add-method-failure")]
    public Task set_get_add_method_failure()
        => ExecutionTestFromFile("set-get-add-method-failure");

    [Fact(DisplayName = "set-iterable")]
    public Task set_iterable()
        => ExecutionTestFromFile("set-iterable");

    [Fact(DisplayName = "set-iterable-calls-add")]
    public Task set_iterable_calls_add()
        => ExecutionTestFromFile("set-iterable-calls-add");

    [Fact(DisplayName = "set-no-iterable")]
    public Task set_no_iterable()
        => ExecutionTestFromFile("set-no-iterable");

    [Fact(DisplayName = "set-undefined-newtarget")]
    public Task set_undefined_newtarget()
        => ExecutionTestFromFile("set-undefined-newtarget");
}
