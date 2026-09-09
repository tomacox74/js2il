using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.AsyncDisposableStack;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.AsyncDisposableStack") { }

    [Fact(DisplayName = "constructor.js")]
    public Task constructor() => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "instance-extensible.js")]
    public Task instance_extensible() => ExecutionTestFromFile("instance-extensible");

    [Fact(DisplayName = "is-a-constructor.js")]
    public Task is_a_constructor() => ExecutionTestFromFile("is-a-constructor");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "newtarget-prototype-is-not-object.js")]
    public Task newtarget_prototype_is_not_object() => ExecutionTestFromFile("newtarget-prototype-is-not-object");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto.js")]
    public Task proto() => ExecutionTestFromFile("proto");

    [Fact(DisplayName = "prototype-from-newtarget-abrupt.js")]
    public Task prototype_from_newtarget_abrupt() => ExecutionTestFromFile("prototype-from-newtarget-abrupt");

    [Fact(DisplayName = "prototype-from-newtarget-custom.js")]
    public Task prototype_from_newtarget_custom() => ExecutionTestFromFile("prototype-from-newtarget-custom");

    [Fact(DisplayName = "prototype-from-newtarget.js")]
    public Task prototype_from_newtarget() => ExecutionTestFromFile("prototype-from-newtarget");

    [Fact(DisplayName = "undefined-newtarget-throws.js")]
    public Task undefined_newtarget_throws() => ExecutionTestFromFile("undefined-newtarget-throws");
}
