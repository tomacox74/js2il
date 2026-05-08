using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.class_.definition;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.class_.definition") { }

    [Fact(DisplayName = "accessors", Skip = "Class accessor definition semantics are incomplete.")]
    public Task accessors()
        => ExecutionTest("accessors");

    [Fact(DisplayName = "basics", Skip = "Class constructor prototype/name semantics are incomplete; Object.getPrototypeOf(C.prototype) currently observes undefined.")]
    public Task basics()
        => ExecutionTest("basics");
}
