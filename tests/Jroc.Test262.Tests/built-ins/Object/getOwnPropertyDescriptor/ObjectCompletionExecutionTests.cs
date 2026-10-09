namespace Jroc.Test262.Tests.built_ins.Object.getOwnPropertyDescriptor;

public class ObjectCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public ObjectCompletionExecutionTests() : base("built_ins.Object.getOwnPropertyDescriptor") { }

    [Fact(DisplayName = "15.2.3.3-4-4.js", Skip = "eval is not supported.")]
    public Task _15_2_3_3_4_4() => ExecutionTestFromFile("15.2.3.3-4-4");

    [Fact(DisplayName = "15.2.3.3-4-51.js")]
    public Task _15_2_3_3_4_51() => ExecutionTestFromFile("15.2.3.3-4-51");
}
