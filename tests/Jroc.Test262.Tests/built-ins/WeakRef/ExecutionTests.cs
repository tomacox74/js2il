using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakRef;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.WeakRef") { }

    [Fact(DisplayName = "constructor.js")]
    public Task constructor() => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "is-a-constructor.js")]
    public Task is_a_constructor() => ExecutionTestFromFile("is-a-constructor");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "newtarget-prototype-is-not-object.js")]
    public Task newtarget_prototype_is_not_object()
        => ExecutionTestFromFile("newtarget-prototype-is-not-object");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto.js")]
    public Task proto() => ExecutionTestFromFile("proto");

    [Fact(DisplayName = "throws-when-target-cannot-be-held-weakly.js")]
    public Task throws_when_target_cannot_be_held_weakly()
        => ExecutionTestFromFile("throws-when-target-cannot-be-held-weakly");

    [Fact(DisplayName = "undefined-newtarget-throws.js")]
    public Task undefined_newtarget_throws() => ExecutionTestFromFile("undefined-newtarget-throws");
}
