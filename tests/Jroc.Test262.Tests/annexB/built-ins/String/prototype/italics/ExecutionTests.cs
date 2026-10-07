using Xunit;

namespace Jroc.Test262.Tests.annexB.built_ins.String.prototype.italics;

public class ExecutionTests : Jroc.Test262.Tests.DiskExecutionTestsBase
{
    public ExecutionTests() : base("annexB/built-ins/String/prototype/italics")
    {
    }

    [Fact(DisplayName = "B.2.3.9.js")]
    public Task B_2_3_9() => ExecutionTest("B.2.3.9");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTest("length");
}
