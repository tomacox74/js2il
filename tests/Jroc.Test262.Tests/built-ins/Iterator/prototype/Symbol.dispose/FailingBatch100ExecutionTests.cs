using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Iterator.prototype.Symbol.dispose;

public class FailingBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public FailingBatch100ExecutionTests() : base("built_ins.Iterator.prototype.Symbol.dispose") { }

    [Fact(DisplayName = "invokes-return")]
    public Task invokes_return() => ExecutionTestFromFile("invokes-return");

    [Fact(DisplayName = "is-function")]
    public Task is_function() => ExecutionTestFromFile("is-function");

    [Fact(DisplayName = "length")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "return-val")]
    public Task return_val() => ExecutionTestFromFile("return-val");

}
