using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.ctors;

public class TypedArrayConstructorsConformanceBatchExecutionTests : InMemoryExecutionTestsBase
{
    public TypedArrayConstructorsConformanceBatchExecutionTests() : base("built_ins.TypedArrayConstructors.ctors") { }

    [Fact(DisplayName = "no-species.js")]
    public Task no_species() => ExecutionTestFromFile("no-species");

}
