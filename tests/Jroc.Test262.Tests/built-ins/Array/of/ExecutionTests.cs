using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.of;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.of") { }

    [Fact(DisplayName = "creates-a-new-array-from-arguments")]
    public Task creates_a_new_array_from_arguments()
        => ExecutionTestFromFile("creates-a-new-array-from-arguments");
}
