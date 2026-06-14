using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Symbol.prototype.description;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Symbol.prototype.description") { }

    [Fact(DisplayName = "description-symboldescriptivestring")]
    public Task description_symboldescriptivestring()
        => ExecutionTestFromFile("description-symboldescriptivestring");

    [Fact(DisplayName = "get")]
    public Task get()
        => ExecutionTestFromFile("get");
}
