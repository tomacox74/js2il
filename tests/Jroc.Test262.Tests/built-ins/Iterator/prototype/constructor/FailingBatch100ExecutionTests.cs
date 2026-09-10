using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator.prototype.constructor;

public class FailingBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Iterator.prototype.constructor") { }

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "weird-setter")]
    public Task weird_setter() => ExecutionTestFromFile("weird-setter");

}
