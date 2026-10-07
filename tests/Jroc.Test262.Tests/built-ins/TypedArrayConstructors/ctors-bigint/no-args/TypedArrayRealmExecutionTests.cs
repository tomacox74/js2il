using Jroc.Test262.Tests.built_ins;

namespace Jroc.Test262.Tests.built_ins.TypedArrayConstructors.ctors_bigint.no_args;

public class TypedArrayRealmExecutionTests : InMemoryExecutionTestsBase
{
    public TypedArrayRealmExecutionTests() : base("built_ins.TypedArrayConstructors.ctors_bigint.no_args") { }

    [Fact(DisplayName = "proto-from-ctor-realm.js")]
    public Task proto_from_ctor_realm() => ExecutionTestFromFile("proto-from-ctor-realm");
}
