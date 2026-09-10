using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DisposableStack;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.DisposableStack") { }

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "newtarget-prototype-is-not-object")]
    public Task newtarget_prototype_is_not_object() => ExecutionTestFromFile("newtarget-prototype-is-not-object");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto")]
    public Task proto() => ExecutionTestFromFile("proto");

    [Fact(DisplayName = "prototype-from-newtarget-abrupt")]
    public Task prototype_from_newtarget_abrupt() => ExecutionTestFromFile("prototype-from-newtarget-abrupt");

    [Fact(DisplayName = "prototype-from-newtarget-custom")]
    public Task prototype_from_newtarget_custom() => ExecutionTestFromFile("prototype-from-newtarget-custom");

    [Fact(DisplayName = "prototype-from-newtarget")]
    public Task prototype_from_newtarget() => ExecutionTestFromFile("prototype-from-newtarget");

    [Fact(DisplayName = "undefined-newtarget-throws")]
    public Task undefined_newtarget_throws() => ExecutionTestFromFile("undefined-newtarget-throws");
}
