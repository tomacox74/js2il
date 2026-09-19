using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Reflect.construct;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Reflect.construct") { }

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

    [Fact(DisplayName = "newtarget-is-not-constructor-throws")]
    public Task newtarget_is_not_constructor_throws()
        => ExecutionTestFromFile("newtarget-is-not-constructor-throws");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor()
        => ExecutionTestFromFile("not-a-constructor");

    [Fact(DisplayName = "return-without-newtarget-argument")]
    public Task return_without_newtarget_argument()
        => ExecutionTestFromFile("return-without-newtarget-argument");

}
