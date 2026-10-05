using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.Boolean;

public class CrossRealmConstructorExecutionTests : InMemoryExecutionTestsBase
{
    public CrossRealmConstructorExecutionTests() : base("built_ins.Boolean") { }

    [Fact(DisplayName = "proto-from-ctor-realm.js")]
    public Task proto_from_ctor_realm() => ExecutionTestFromFile("proto-from-ctor-realm");
}
