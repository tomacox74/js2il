using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.slice;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.Array.prototype.slice") { }

    [Fact(DisplayName = "coerced-start-end-grow.js")]
    public Task test_coerced_start_end_grow() => ExecutionTest("coerced-start-end-grow");

    [Fact(DisplayName = "coerced-start-end-shrink.js")]
    public Task test_coerced_start_end_shrink() => ExecutionTest("coerced-start-end-shrink");

    [Fact(DisplayName = "create-proto-from-ctor-realm-non-array.js")]
    public Task test_create_proto_from_ctor_realm_non_array() => ExecutionTest("create-proto-from-ctor-realm-non-array");
}
