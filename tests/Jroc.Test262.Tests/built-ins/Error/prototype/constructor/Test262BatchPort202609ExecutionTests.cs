using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Error.prototype.constructor;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Error.prototype.constructor") { }

    [Fact(DisplayName = "S15.11.4.1_A1_T2")]
    public Task S15_11_4_1_A1_T2()
        => ExecutionTestFromFile("S15.11.4.1_A1_T2");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

}
