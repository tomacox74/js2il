using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Reflect.apply;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Reflect.apply") { }

    [Fact(DisplayName = "arguments-list-is-not-array-like")]
    public Task arguments_list_is_not_array_like()
        => ExecutionTestFromFile("arguments-list-is-not-array-like");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

}
