using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.Symbol.unscopables;

public class ExecutionTests : InMemoryExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Array.prototype.Symbol.unscopables") { }

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc()
        => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "value")]
    public Task value()
        => ExecutionTestFromFile("value");

    [Fact(DisplayName = "array-find-from-last")]
    public Task array_find_from_last()
        => ExecutionTestFromFile("array-find-from-last");

    [Fact(DisplayName = "change-array-by-copy")]
    public Task change_array_by_copy()
        => ExecutionTestFromFile("change-array-by-copy");
}
