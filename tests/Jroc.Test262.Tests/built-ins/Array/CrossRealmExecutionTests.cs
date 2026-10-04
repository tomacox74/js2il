namespace Jroc.Test262.Tests.built_ins.Array;

public partial class ExecutionTests
{
    [Fact(DisplayName = "proto-from-ctor-realm-one.js")]
    public Task proto_from_ctor_realm_one()
        => ExecutionTestFromFile("proto-from-ctor-realm-one");

    [Fact(DisplayName = "proto-from-ctor-realm-two.js")]
    public Task proto_from_ctor_realm_two()
        => ExecutionTestFromFile("proto-from-ctor-realm-two");

    [Fact(DisplayName = "proto-from-ctor-realm-zero.js")]
    public Task proto_from_ctor_realm_zero()
        => ExecutionTestFromFile("proto-from-ctor-realm-zero");
}
