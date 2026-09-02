using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.BigUint64Array.prototype;

public class TypedArrayConstructorsConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConstructorsConformanceBatchExecutionTests() : base("built_ins.TypedArrayConstructors.BigUint64Array.prototype") { }

    [Fact(DisplayName = "BYTES_PER_ELEMENT.js")]
    public Task BYTES_PER_ELEMENT() => ExecutionTestFromFile("BYTES_PER_ELEMENT");

    [Fact(DisplayName = "constructor.js")]
    public Task constructor() => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "not-typedarray-object.js")]
    public Task not_typedarray_object() => ExecutionTestFromFile("not-typedarray-object");

    [Fact(DisplayName = "proto.js")]
    public Task proto() => ExecutionTestFromFile("proto");

}
