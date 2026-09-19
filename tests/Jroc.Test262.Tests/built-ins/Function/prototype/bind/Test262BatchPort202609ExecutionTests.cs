using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Function.prototype.bind;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Function.prototype.bind") { }

    [Fact(DisplayName = "S15.3.4.5_A5")]
    public Task S15_3_4_5_A5()
        => ExecutionTestFromFile("S15.3.4.5_A5");

}
