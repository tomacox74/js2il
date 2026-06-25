using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.slice;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("Array.prototype.slice") { }

    [Fact(DisplayName = "15.4.4.10-10-c-ii-1")]
    public Task _15_4_4_10_10_c_ii_1()
        => ExecutionTestFromFile("15.4.4.10-10-c-ii-1");
}
