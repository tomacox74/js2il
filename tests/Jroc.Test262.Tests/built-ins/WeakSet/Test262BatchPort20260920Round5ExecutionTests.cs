using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakSet;

public class Test262BatchPort20260920Round5ExecutionTests : InMemoryExecutionTestsBase
{
    public Test262BatchPort20260920Round5ExecutionTests() : base("built_ins.WeakSet") { }

    [Fact(DisplayName = "iterable-with-symbol-values")]
    public Task iterable_with_symbol_values()
        => ExecutionTestFromFile("iterable-with-symbol-values");

}
