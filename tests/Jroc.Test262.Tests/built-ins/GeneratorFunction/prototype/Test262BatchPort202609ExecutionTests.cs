using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.GeneratorFunction.prototype;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.GeneratorFunction.prototype") { }

    [Fact(DisplayName = "not-callable")]
    public Task not_callable()
        => ExecutionTestFromFile("not-callable");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

}
