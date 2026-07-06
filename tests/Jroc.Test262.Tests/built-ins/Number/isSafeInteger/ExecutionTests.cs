using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Number.isSafeInteger;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Number.isSafeInteger") { }

    [Fact(DisplayName = "not-safe-integer")]
    public Task not_safe_integer()
        => ExecutionTestFromFile("not-safe-integer");

    [Fact(DisplayName = "safe-integers")]
    public Task safe_integers()
        => ExecutionTestFromFile("safe-integers");
}
