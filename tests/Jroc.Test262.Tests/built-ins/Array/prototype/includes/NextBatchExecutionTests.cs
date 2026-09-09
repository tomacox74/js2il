using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.includes;

public class NextBatchExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.Array.prototype.includes") { }

    [Fact(DisplayName = "coerced-searchelement-fromindex-resize")]
    public Task coerced_searchelement_fromindex_resize() => ExecutionTestFromFile("coerced-searchelement-fromindex-resize");

    [Fact(DisplayName = "no-arg")]
    public Task no_arg() => ExecutionTestFromFile("no-arg");

    [Fact(DisplayName = "samevaluezero")]
    public Task samevaluezero() => ExecutionTestFromFile("samevaluezero");

}
