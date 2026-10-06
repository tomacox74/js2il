using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.WeakRef;

public class ConstructorRealmBatchExecutionTests : InMemoryExecutionTestsBase
{
    public ConstructorRealmBatchExecutionTests() : base("built_ins.WeakRef") { }

    [Fact(DisplayName = "proto-from-ctor-realm.js")]
    public Task proto_from_ctor_realm() => ExecutionTestFromFile("proto-from-ctor-realm");
}
