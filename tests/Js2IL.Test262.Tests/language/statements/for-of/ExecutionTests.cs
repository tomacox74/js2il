using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.statements.for_of;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.statements.for_of") { }

    [Fact(DisplayName = "ArgumentsObject_mapped-parameter-aliasing-for-of")]
    public Task ArgumentsObject_mapped_parameter_aliasing_for_of()
        => ExecutionTest("ArgumentsObject_mapped-parameter-aliasing-for-of");
}
