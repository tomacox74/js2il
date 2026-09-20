using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakRef;

public class Test262BatchPort20260920Round5ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260920Round5ExecutionTests() : base("built_ins.WeakRef") { }

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

    [Fact(DisplayName = "returns-new-object-from-constructor-with-object-target")]
    public Task returns_new_object_from_constructor_with_object_target()
        => ExecutionTestFromFile("returns-new-object-from-constructor-with-object-target");

    [Fact(DisplayName = "returns-new-object-from-constructor-with-symbol-target")]
    public Task returns_new_object_from_constructor_with_symbol_target()
        => ExecutionTestFromFile("returns-new-object-from-constructor-with-symbol-target");

}
