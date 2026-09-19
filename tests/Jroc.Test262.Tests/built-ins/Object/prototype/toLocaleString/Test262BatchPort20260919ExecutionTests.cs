using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.prototype.toLocaleString;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Object.prototype.toLocaleString") { }

    [Fact(DisplayName = "primitive_this_value")]
    public Task primitive_this_value()
        => ExecutionTestFromFile("primitive_this_value");

    [Fact(DisplayName = "primitive_this_value_getter")]
    public Task primitive_this_value_getter()
        => ExecutionTestFromFile("primitive_this_value_getter");

}
