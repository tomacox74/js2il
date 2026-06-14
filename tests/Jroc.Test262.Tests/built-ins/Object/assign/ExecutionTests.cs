using Jroc.Tests;

namespace Jroc.Test262.Tests.built_ins.Object.assign;

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

    [Fact(DisplayName = "assign-descriptor")]
    public Task assign_descriptor()
        => ExecutionTest("assign-descriptor");

    [Fact(DisplayName = "assign-length")]
    public Task assign_length()
        => ExecutionTest("assign-length");

    [Fact(DisplayName = "assignment-to-readonly-property-of-target-must-throw-a-typeerror-exception")]
    public Task assignment_to_readonly_property_of_target_must_throw_a_typeerror_exception()
        => ExecutionTest("assignment-to-readonly-property-of-target-must-throw-a-typeerror-exception");

    [Fact(DisplayName = "invoked-as-ctor")]
    public Task invoked_as_ctor()
        => ExecutionTest("invoked-as-ctor");

}
