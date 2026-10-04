namespace Jroc.Test262.Tests.built_ins.Array.prototype.map;

public partial class ExecutionTests
{
    [Fact(DisplayName = "create-proto-from-ctor-realm-array.js")]
    public Task create_proto_from_ctor_realm_array()
        => ExecutionTestFromFile("create-proto-from-ctor-realm-array");
}
