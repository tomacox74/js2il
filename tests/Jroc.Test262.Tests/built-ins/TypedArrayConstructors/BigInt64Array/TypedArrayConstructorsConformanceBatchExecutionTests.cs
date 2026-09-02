using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.BigInt64Array;

public class TypedArrayConstructorsConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConstructorsConformanceBatchExecutionTests() : base("built_ins.TypedArrayConstructors.BigInt64Array") { }

    [Fact(DisplayName = "BYTES_PER_ELEMENT.js")]
    public Task BYTES_PER_ELEMENT() => ExecutionTestFromFile("BYTES_PER_ELEMENT");

    [Fact(DisplayName = "constructor.js")]
    public Task constructor() => ExecutionTestFromFile("constructor");

    [Fact(DisplayName = "is-a-constructor.js")]
    public Task is_a_constructor() => ExecutionTestFromFile("is-a-constructor");

    [Fact(DisplayName = "length.js")]
    public Task length() => ExecutionTestFromFile("length");

    [Fact(DisplayName = "name.js")]
    public Task name() => ExecutionTestFromFile("name");

    [Fact(DisplayName = "prop-desc.js")]
    public Task prop_desc() => ExecutionTestFromFile("prop-desc");

    [Fact(DisplayName = "proto.js")]
    public Task proto() => ExecutionTestFromFile("proto");

    [Fact(DisplayName = "prototype.js")]
    public Task prototype() => ExecutionTestFromFile("prototype");

}
