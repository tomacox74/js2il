using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.allSettled;

public class RejectionReasonExecutionTests : InMemoryExecutionTestsBase
{
    public RejectionReasonExecutionTests() : base("built_ins.Promise.allSettled") { }

    [Fact(DisplayName = "resolved-then-catch-finally.js")]
    public Task resolved_then_catch_finally()
        => ExecutionTestFromFile("resolved-then-catch-finally");
}
