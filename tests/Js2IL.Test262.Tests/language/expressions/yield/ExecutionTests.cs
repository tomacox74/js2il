using Js2IL.Test262.Tests.language;

namespace Js2IL.Test262.Tests.language.expressions.yield_;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("language.expressions.yield") { }

    [Fact(DisplayName = "captured-free-vars", Skip = "Blocked by existing generator scope capture issue.")]
    public Task captured_free_vars()
        => ExecutionTest("captured-free-vars");

    [Fact(DisplayName = "formal-parameters")]
    public Task formal_parameters()
        => ExecutionTest("formal-parameters");

    [Fact(DisplayName = "from-catch")]
    public Task from_catch()
        => ExecutionTest("from-catch");

    [Fact(DisplayName = "from-try")]
    public Task from_try()
        => ExecutionTest("from-try");

    [Fact(DisplayName = "rhs-yield")]
    public Task rhs_yield()
        => ExecutionTest("rhs-yield");

    [Fact(DisplayName = "star-array")]
    public Task star_array()
        => ExecutionTest("star-array");

    [Fact(DisplayName = "star-iterable", Skip = "Blocked by current generator iterable local-storage compilation issue.")]
    public Task star_iterable()
        => ExecutionTest("star-iterable");

}
