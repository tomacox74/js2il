using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Reflect.construct;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Reflect.construct") { }

    [Fact(DisplayName = "arguments-list-is-not-array-like")]
    public Task arguments_list_is_not_array_like()
        => ExecutionTestFromFile("arguments-list-is-not-array-like");

    [Fact(DisplayName = "length")]
    public Task length()
        => ExecutionTestFromFile("length");
}
