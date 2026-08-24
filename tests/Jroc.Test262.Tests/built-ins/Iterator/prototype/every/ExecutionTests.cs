using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator.prototype.every;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Iterator.prototype.every") { }

    [Fact(DisplayName = "name")]
    public Task name()
        => ExecutionTestFromFile("name");
}
