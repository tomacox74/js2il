using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.FinalizationRegistry;

public class Test262BatchPort202609ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort202609ExecutionTests() : base("built_ins.FinalizationRegistry") { }

    [Fact(DisplayName = "instance-extensible")]
    public Task instance_extensible()
        => ExecutionTestFromFile("instance-extensible");

    [Fact(DisplayName = "prototype-from-newtarget-abrupt")]
    public Task prototype_from_newtarget_abrupt()
        => ExecutionTestFromFile("prototype-from-newtarget-abrupt");

    [Fact(DisplayName = "prototype-from-newtarget-custom")]
    public Task prototype_from_newtarget_custom()
        => ExecutionTestFromFile("prototype-from-newtarget-custom");

    [Fact(DisplayName = "prototype-from-newtarget")]
    public Task prototype_from_newtarget()
        => ExecutionTestFromFile("prototype-from-newtarget");

    [Fact(DisplayName = "target-not-callable-throws")]
    public Task target_not_callable_throws()
        => ExecutionTestFromFile("target-not-callable-throws");

    [Fact(DisplayName = "unnaffected-by-poisoned-cleanupCallback")]
    public Task unnaffected_by_poisoned_cleanupCallback()
        => ExecutionTestFromFile("unnaffected-by-poisoned-cleanupCallback");

}
