using Js2IL.Test262.Tests.built_ins;


namespace Js2IL.Test262.Tests.built_ins.TypedArray.from;


public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.TypedArray.from") { }

    [Fact(DisplayName = "iterated-array-changed-by-tonumber")]
    public Task iterated_array_changed_by_tonumber()
        => ExecutionTestFromFile("iterated-array-changed-by-tonumber");
}
