using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.concat;

public class NextBatchExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.Array.prototype.concat") { }

    [Fact(DisplayName = "create-proto-from-ctor-realm-non-array")]
    public Task create_proto_from_ctor_realm_non_array() => ExecutionTestFromFile("create-proto-from-ctor-realm-non-array");

}
