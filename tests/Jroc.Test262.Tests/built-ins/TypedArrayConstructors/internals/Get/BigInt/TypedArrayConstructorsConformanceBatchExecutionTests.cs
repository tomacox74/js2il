using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.internals.Get.BigInt;

public class TypedArrayConstructorsConformanceBatchExecutionTests : DiskExecutionTestsBase
{
    public TypedArrayConstructorsConformanceBatchExecutionTests() : base("built_ins.TypedArrayConstructors.internals.Get.BigInt") { }

    [Fact(DisplayName = "indexed-value-sab.js")]
    public Task indexed_value_sab() => ExecutionTestFromFile("indexed-value-sab");

    [Fact(DisplayName = "indexed-value.js")]
    public Task indexed_value() => ExecutionTestFromFile("indexed-value");

    [Fact(DisplayName = "key-is-not-canonical-index.js")]
    public Task key_is_not_canonical_index() => ExecutionTestFromFile("key-is-not-canonical-index");

}
