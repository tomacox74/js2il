using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.reverse;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.Array.prototype.reverse") { }

    [Fact(DisplayName = "length-exceeding-integer-limit-with-proxy")]
    public Task length_exceeding_integer_limit_with_proxy()
        => ExecutionTestFromFile("length-exceeding-integer-limit-with-proxy");

}
