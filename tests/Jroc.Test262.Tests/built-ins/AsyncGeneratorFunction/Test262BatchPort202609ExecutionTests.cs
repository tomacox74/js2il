using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncGeneratorFunction;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.AsyncGeneratorFunction") { }

    [Fact(DisplayName = "instance-construct-throws")]
    public Task instance_construct_throws()
        => ExecutionTestFromFile("instance-construct-throws");

    [Fact(DisplayName = "instance-name")]
    public Task instance_name()
        => ExecutionTestFromFile("instance-name");

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");

}
