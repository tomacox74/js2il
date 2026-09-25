using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.isFrozen;

public partial class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.isFrozen") { }

    [Fact(DisplayName = "15.2.3.12-1-1.js")]
    public Task _15_2_3_12_1_1_js()
        => ExecutionTestFromFile("15.2.3.12-1-1");

    [Fact(DisplayName = "15.2.3.12-1-2.js")]
    public Task _15_2_3_12_1_2_js()
        => ExecutionTestFromFile("15.2.3.12-1-2");
}
