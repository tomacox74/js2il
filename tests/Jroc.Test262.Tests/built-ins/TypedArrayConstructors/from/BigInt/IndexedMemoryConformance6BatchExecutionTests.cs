using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.@from.BigInt;

public class IndexedMemoryConformance6BatchExecutionTests : DiskExecutionTestsBase
{
    public IndexedMemoryConformance6BatchExecutionTests() : base("built_ins.TypedArrayConstructors.from.BigInt") { }

    [Fact(DisplayName = "mapfn-arguments.js")]
    public Task mapfn_arguments() => ExecutionTestFromFile("mapfn-arguments");

    [Fact(DisplayName = "mapfn-this-with-thisarg.js")]
    public Task mapfn_this_with_thisarg() => ExecutionTestFromFile("mapfn-this-with-thisarg");

    [Fact(DisplayName = "mapfn-this-without-thisarg-non-strict.js")]
    public Task mapfn_this_without_thisarg_non_strict() => ExecutionTestFromFile("mapfn-this-without-thisarg-non-strict");

    [Fact(DisplayName = "mapfn-this-without-thisarg-strict.js")]
    public Task mapfn_this_without_thisarg_strict() => ExecutionTestFromFile("mapfn-this-without-thisarg-strict");

    [Fact(DisplayName = "new-instance-from-ordinary-object.js")]
    public Task new_instance_from_ordinary_object() => ExecutionTestFromFile("new-instance-from-ordinary-object");

    [Fact(DisplayName = "new-instance-using-custom-ctor.js")]
    public Task new_instance_using_custom_ctor() => ExecutionTestFromFile("new-instance-using-custom-ctor");

    [Fact(DisplayName = "new-instance-with-mapfn.js")]
    public Task new_instance_with_mapfn() => ExecutionTestFromFile("new-instance-with-mapfn");

    [Fact(DisplayName = "new-instance-without-mapfn.js")]
    public Task new_instance_without_mapfn() => ExecutionTestFromFile("new-instance-without-mapfn");

    [Fact(DisplayName = "set-value-abrupt-completion.js")]
    public Task set_value_abrupt_completion() => ExecutionTestFromFile("set-value-abrupt-completion");
}
