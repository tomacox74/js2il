using Js2IL.Tests;

namespace Js2IL.Test262.Tests.language.arguments_object;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("language.arguments_object") { }

    [Fact(DisplayName = "ArgumentsObject_callee-descriptor-non-strict")]
    public Task ArgumentsObject_callee_descriptor_non_strict()
        => ExecutionTest("ArgumentsObject_callee-descriptor-non-strict");
}
