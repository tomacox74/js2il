using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Symbol.split;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Symbol.split") { }

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

}
