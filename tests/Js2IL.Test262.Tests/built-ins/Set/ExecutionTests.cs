using Js2IL.Test262.Tests.built_ins;

namespace Js2IL.Test262.Tests.built_ins.Set;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Set") { }

    [Fact(DisplayName = "bigint-number-same-value")]
    public Task bigint_number_same_value()
        => ExecutionTestFromFile("bigint-number-same-value");

    [Fact(DisplayName = "constructor")]
    public Task constructor()
        => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "properties-of-the-set-prototype-object")]
    public Task properties_of_the_set_prototype_object()
        => ExecutionTestFromFile("properties-of-the-set-prototype-object");

    [Fact(DisplayName = "set-iterable")]
    public Task set_iterable()
        => ExecutionTestFromFile("set-iterable");

    [Fact(DisplayName = "set-no-iterable")]
    public Task set_no_iterable()
        => ExecutionTestFromFile("set-no-iterable");
}
