using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Object.assign;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.assign") { }

    [Fact(DisplayName = "ObjectOverride-sameproperty")]
    public Task ObjectOverride_sameproperty()
        => ExecutionTest("ObjectOverride-sameproperty");

    [Fact(DisplayName = "OnlyOneArgument")]
    public Task OnlyOneArgument()
        => ExecutionTest("OnlyOneArgument");

    [Fact(DisplayName = "Override-notstringtarget")]
    public Task Override_notstringtarget()
        => ExecutionTest("Override-notstringtarget");

    [Fact(DisplayName = "Override")]
    public Task Override()
        => ExecutionTest("Override");
}
