using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArray.prototype.toSorted;

public class ExecutionTests : DiskExecutionTestsBase
{
    public ExecutionTests() : base("TypedArray.prototype.toSorted") { }

    [Fact(DisplayName = "comparefn-controls-sort")]
    public Task comparefn_controls_sort() => ExecutionTestFromFile("comparefn-controls-sort");

    [Fact(DisplayName = "comparefn-default")]
    public Task comparefn_default() => ExecutionTestFromFile("comparefn-default");

    [Fact(DisplayName = "comparefn-not-a-function")]
    public Task comparefn_not_a_function() => ExecutionTestFromFile("comparefn-not-a-function");

    [Fact(DisplayName = "comparefn-stop-after-error")]
    public Task comparefn_stop_after_error() => ExecutionTestFromFile("comparefn-stop-after-error");

    [Fact(DisplayName = "immutable")]
    public Task immutable() => ExecutionTestFromFile("immutable");

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "not-a-constructor")]
    public Task not_a_constructor() => ExecutionTestFromFile("not-a-constructor");
}
