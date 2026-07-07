using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.BigInt.prototype.valueOf;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.BigInt.prototype.valueOf") { }

    [Fact(DisplayName = "return")]
    public Task @return()
        => ExecutionTestFromFile("return");

    [Fact(DisplayName = "this-value-invalid-object-throws")]
    public Task this_value_invalid_object_throws()
        => ExecutionTestFromFile("this-value-invalid-object-throws");
}
