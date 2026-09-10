using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Promise.allSettledKeyed;

public class NextBatch100ExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatch100ExecutionTests() : base("built_ins.Promise.allSettledKeyed") { }

    [Fact(DisplayName = "ctx-non-ctor")]
    public Task ctx_non_ctor() => ExecutionTestFromFile("ctx-non-ctor");
}
