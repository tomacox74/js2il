using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Set.prototype.forEach;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("built_ins.Set.prototype.forEach") { }

    [Fact(DisplayName = "forEach")]
    public Task forEach()
        => ExecutionTestFromFile("forEach");

    [Fact(DisplayName = "iterates-in-insertion-order")]
    public Task iterates_in_insertion_order()
        => ExecutionTestFromFile("iterates-in-insertion-order");

    [Fact(DisplayName = "returns-undefined")]
    public Task returns_undefined()
        => ExecutionTestFromFile("returns-undefined");

    [Fact(DisplayName = "iterates-values-added-after-foreach-begins")]
    public Task iterates_values_added_after_foreach_begins()
        => ExecutionTestFromFile("iterates-values-added-after-foreach-begins");

}
