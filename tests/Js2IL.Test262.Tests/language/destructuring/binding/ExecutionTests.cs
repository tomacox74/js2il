using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.destructuring.binding;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.destructuring.binding") { }

    [Fact(DisplayName = "initialization-returns-normal-completion-for-empty-objects")]
    public Task initialization_returns_normal_completion_for_empty_objects()
        => ExecutionTest("initialization-returns-normal-completion-for-empty-objects");
}
