using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Array.prototype.filter;

public class NextBatchExecutionTests : InMemoryExecutionTestsBase
{
    public NextBatchExecutionTests() : base("built_ins.Array.prototype.filter") { }

    [Fact(DisplayName = "15.4.4.20-5-7")]
    public Task _15_4_4_20_5_7() => ExecutionTestFromFile("15.4.4.20-5-7");

    [Fact(DisplayName = "create-proto-from-ctor-realm-non-array")]
    public Task create_proto_from_ctor_realm_non_array() => ExecutionTestFromFile("create-proto-from-ctor-realm-non-array");

}
