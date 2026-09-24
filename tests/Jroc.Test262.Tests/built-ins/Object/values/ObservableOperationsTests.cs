using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Object.values;

public partial class ExecutionTests
{
    [Fact(DisplayName = "observable-operations.js")]
    public Task observable_operations()
        => ExecutionTestFromFile("observable-operations");
}
