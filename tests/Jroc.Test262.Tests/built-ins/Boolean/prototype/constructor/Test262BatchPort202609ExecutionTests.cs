using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Boolean.prototype.constructor;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Boolean.prototype.constructor") { }

    [Fact(DisplayName = "S15.6.4.1_A1")]
    public Task S15_6_4_1_A1()
        => ExecutionTestFromFile("S15.6.4.1_A1");

}
