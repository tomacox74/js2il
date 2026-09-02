using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.Int8Array;

public class TypedArrayConstructorsConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConstructorsConformanceBatchExecutionTests() : base("built_ins.TypedArrayConstructors.Int8Array") { }

    [Fact(DisplayName = "BYTES_PER_ELEMENT.js")]
    public Task BYTES_PER_ELEMENT() => ExecutionTestFromFile("BYTES_PER_ELEMENT");

    [Fact(DisplayName = "is-a-constructor.js")]
    public Task is_a_constructor() => ExecutionTestFromFile("is-a-constructor");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prototype.js")]
    public Task prototype() => ExecutionTestFromFile("prototype");

}
