using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.DataView;

public class CrossRealmConstructorExecutionTests : InMemoryExecutionTestsBase
{
    public CrossRealmConstructorExecutionTests() : base("built_ins.DataView") { }

    [Fact(DisplayName = "proto-from-ctor-realm-sab.js")]
    public Task proto_from_ctor_realm_sab() => ExecutionTestFromFile("proto-from-ctor-realm-sab");

    [Fact(DisplayName = "proto-from-ctor-realm.js")]
    public Task proto_from_ctor_realm() => ExecutionTestFromFile("proto-from-ctor-realm");
}
