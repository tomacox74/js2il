using Js2IL.Tests;

namespace Js2IL.Test262.Tests.built_ins.Object.assign;

public class ExecutionTests : ExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Object.assign") { }

    [Fact(DisplayName = "ObjectOverride-sameproperty", Skip = "Known JS2IL defect")]
    public Task ObjectOverride_sameproperty()
        => ExecutionTest("ObjectOverride-sameproperty");

    [Fact(DisplayName = "OnlyOneArgument", Skip = "Known JS2IL defect")]
    public Task OnlyOneArgument()
        => ExecutionTest("OnlyOneArgument");

    [Fact(DisplayName = "Override-notstringtarget", Skip = "Known JS2IL defect")]
    public Task Override_notstringtarget()
        => ExecutionTest("Override-notstringtarget");

    [Fact(DisplayName = "Override", Skip = "Known JS2IL defect")]
    public Task Override()
        => ExecutionTest("Override");
}
