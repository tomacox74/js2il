using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.map;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.Array.prototype.map") { }

    [Fact(DisplayName = "create-proto-from-ctor-realm-non-array.js")]
    public Task test_create_proto_from_ctor_realm_non_array() => ExecutionTest("create-proto-from-ctor-realm-non-array");
}
