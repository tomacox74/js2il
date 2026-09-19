using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Symbol.search;

public class Test262BatchPort20260919ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260919ExecutionTests() : base("built_ins.Symbol.search") { }

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

}
