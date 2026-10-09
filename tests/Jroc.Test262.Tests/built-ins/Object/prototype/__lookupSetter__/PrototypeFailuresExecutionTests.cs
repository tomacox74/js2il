namespace Jroc.Test262.Tests.built_ins.Object.prototype.__lookupSetter__;

public sealed class PrototypeFailuresExecutionTests : InMemoryExecutionTestsBase
{
    public PrototypeFailuresExecutionTests() : base("built_ins.Object.prototype.__lookupSetter__") { }

    [Fact(DisplayName = "built-ins/Object/prototype/__lookupSetter__/lookup-own-proto-err.js")]
    public Task lookup_own_proto_err() => ExecutionTestFromFile("lookup-own-proto-err");

    [Fact(DisplayName = "built-ins/Object/prototype/__lookupSetter__/lookup-proto-proto-err.js")]
    public Task lookup_proto_proto_err() => ExecutionTestFromFile("lookup-proto-proto-err");
}
