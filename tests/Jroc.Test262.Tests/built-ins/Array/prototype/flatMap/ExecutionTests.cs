using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.flatMap;

public partial class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.flatMap") { }

    [Fact(DisplayName = "call-with-boolean")]
    public Task call_with_boolean()
        => ExecutionTestFromFile("call-with-boolean");

    [Fact(DisplayName = "array-like-objects")]
    public Task array_like_objects()
        => ExecutionTestFromFile("array-like-objects");

    [Fact(DisplayName = "thisArg-argument")]
    public Task thisArg_argument()
        => ExecutionTestFromFile("thisArg-argument");
}
