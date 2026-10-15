using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.splice;

public sealed class BuiltinCompletionExecutionTests : InMemoryExecutionTestsBase
{
    public BuiltinCompletionExecutionTests() : base("built_ins.Array.prototype.splice") { }

    [Fact(DisplayName = "create-proto-from-ctor-realm-array.js")]
    public Task test_create_proto_from_ctor_realm_array() => ExecutionTest("create-proto-from-ctor-realm-array");

    [Fact(DisplayName = "create-proto-from-ctor-realm-non-array.js")]
    public Task test_create_proto_from_ctor_realm_non_array() => ExecutionTest("create-proto-from-ctor-realm-non-array");
}
