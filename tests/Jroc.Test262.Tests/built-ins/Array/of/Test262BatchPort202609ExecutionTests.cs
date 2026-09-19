using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.of;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Array.of") { }

    [Fact(DisplayName = "return-a-new-array-object")]
    public Task return_a_new_array_object()
        => ExecutionTestFromFile("return-a-new-array-object");

}
